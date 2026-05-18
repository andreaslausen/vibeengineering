using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpLogging();
app.MapHealthChecks("/health");
app.MapGet("/", () => "Zeiterfassung.API läuft!");

app.Run();
