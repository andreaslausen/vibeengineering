using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Zeiterfassung.API;

public sealed class PostgresConnectionHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public PostgresConnectionHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? _configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return HealthCheckResult.Unhealthy("Connection string 'DefaultConnection' fehlt.");
        }

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            await connection.CloseAsync();
            return HealthCheckResult.Healthy("PostgreSQL erreichbar.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL nicht erreichbar.", ex);
        }
    }
}
