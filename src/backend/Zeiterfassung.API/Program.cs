
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Zeiterfassung.Infrastructure;
using Zeiterfassung.Infrastructure.Repositories;
using Zeiterfassung.Domain.Repositories;
using Zeiterfassung.Application.Services;
using Zeiterfassung.Application.UseCases.Auth;
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

builder.Services.AddControllers();

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
app.MapControllers();

// Root-Endpoint
app.MapGet("/", () => "Zeiterfassung.API läuft!");

app.Run();
