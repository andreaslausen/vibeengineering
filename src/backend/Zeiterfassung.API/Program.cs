
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zeiterfassung.Infrastructure;
using Zeiterfassung.Infrastructure.Repositories;
using Zeiterfassung.Domain.Repositories;
using Zeiterfassung.Application.Services;
using Zeiterfassung.Application.UseCases.Auth;
using Zeiterfassung.Application.Dtos.Auth;
using Zeiterfassung.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// JWT-Konfiguration laden
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() 
    ?? throw new InvalidOperationException("JWT-Konfiguration nicht gefunden.");

// DbContext-Registrierung für EF Core
builder.Services.AddDbContext<ZeiterfassungDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Host=localhost;Database=zeiterfassung;Username=postgres;Password=postgres"));

// Repository-Registrierung
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();

// Service-Registrierung
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton(new JwtTokenService(
    jwtSettings.SecretKey,
    jwtSettings.Issuer,
    jwtSettings.Audience,
    jwtSettings.AccessTokenExpirationMinutes,
    jwtSettings.RefreshTokenExpirationDays));

builder.Services.AddScoped<PasswordService>();

// Use Case-Registrierung
builder.Services.AddScoped<RegisterUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<RefreshTokenUseCase>();
builder.Services.AddScoped<LogoutUseCase>();
builder.Services.AddScoped<GetUserProfileUseCase>();

// JWT-Authentication konfigurieren
var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            NameClaimType = JwtRegisteredClaimNames.UniqueName,
            ClockSkew = TimeSpan.Zero
        };

        // Bearer token aus Authorization-Header extrahieren
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                if (authHeader?.StartsWith("Bearer ") == true)
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestMethod |
        HttpLoggingFields.RequestPath |
        HttpLoggingFields.ResponseStatusCode |
        HttpLoggingFields.Duration;
});

builder.Services.AddHealthChecks()
    .AddCheck("postgresql", new Zeiterfassung.API.PostgresConnectionHealthCheck(builder.Configuration));

var app = builder.Build();

// Migrations ausführen (nur in Entwicklung)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ZeiterfassungDbContext>();
        db.Database.Migrate();
    }
}

app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();

// Health Check
app.MapHealthChecks("/health");

// Root-Endpoint
app.MapGet("/", () => "Zeiterfassung.API läuft!");

// Authentifizierungs-Endpoints
var authGroup = app.MapGroup("/auth").WithTags("Auth");

// POST /auth/register
authGroup.MapPost("/register", async (RegisterRequestDto request, RegisterUseCase useCase, ZeiterfassungDbContext db) =>
{
    try
    {
        var response = await useCase.ExecuteAsync(request);
        await db.SaveChangesAsync();
        return Results.Created($"/auth/me", response);
    }
    catch (InvalidOperationException)
    {
        return Results.BadRequest(new { error = "Benutzername existiert bereits oder Passwort ungültig." });
    }
    catch
    {
        return Results.Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
    }
});

// POST /auth/login
authGroup.MapPost("/login", async (LoginRequestDto request, LoginUseCase useCase, ZeiterfassungDbContext db) =>
{
    try
    {
        var response = await useCase.ExecuteAsync(request);
        await db.SaveChangesAsync();
        return Results.Ok(response);
    }
    catch (InvalidOperationException)
    {
        return Results.Unauthorized();
    }
    catch
    {
        return Results.Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
    }
})
.Produces(200)
.Produces(401)
.WithName("Login")
.WithDescription("Login mit Benutzername und Passwort");

// POST /auth/refresh
authGroup.MapPost("/refresh", async (RefreshTokenRequestDto request, RefreshTokenUseCase useCase, ZeiterfassungDbContext db) =>
{
    try
    {
        var response = await useCase.ExecuteAsync(request);
        await db.SaveChangesAsync();
        return Results.Ok(response);
    }
    catch (InvalidOperationException)
    {
        return Results.Unauthorized();
    }
    catch
    {
        return Results.Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
    }
})
.WithName("RefreshToken")
.WithDescription("Token aktualisieren (Refresh Token Rotation)");

// POST /auth/logout
authGroup.MapPost("/logout", async (LogoutRequestDto request, LogoutUseCase useCase, ZeiterfassungDbContext db) =>
{
    try
    {
        await useCase.ExecuteAsync(request.RefreshToken);
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Erfolgreich abgemeldet." });
    }
    catch (InvalidOperationException)
    {
        return Results.BadRequest(new { error = "Ungültiger Refresh Token." });
    }
    catch
    {
        return Results.Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
    }
})
.RequireAuthorization("AuthenticatedUser")
.WithName("Logout")
.WithDescription("Benutzer abmelden (aktuelle Session beenden)");

// GET /auth/me
authGroup.MapGet("/me", async (GetUserProfileUseCase useCase, HttpContext context) =>
{
    try
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirst("sub");
        if (userIdClaim == null)
        {
            return Results.Unauthorized();
        }

        var response = await useCase.ExecuteAsync(userIdClaim.Value);
        return Results.Ok(response);
    }
    catch (InvalidOperationException)
    {
        return Results.NotFound(new { error = "Benutzer nicht gefunden." });
    }
    catch
    {
        return Results.Problem("Ein Fehler ist aufgetreten.", statusCode: 500);
    }
})
.RequireAuthorization("AuthenticatedUser")
.WithName("GetProfile")
.WithDescription("Benutzerprofil abrufen");

app.Run();
