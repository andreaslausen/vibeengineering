using System.Security.Claims;
using Zeiterfassung.Domain.Repositories;

namespace Zeiterfassung.Application.UseCases.Auth
{
    /// <summary>
    /// Use Case für Logout (nur aktuelle Session beenden)
    /// </summary>
    public class LogoutUseCase
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LogoutUseCase(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        }

        public async Task ExecuteAsync(string refreshToken)
        {
            // Finde den Refresh Token
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (token == null)
                throw new InvalidOperationException("Ungültiger Refresh Token.");

            // Widerrufe den Token (nur für aktuelle Session)
            token.RevokedAt = DateTimeOffset.UtcNow;
            await _refreshTokenRepository.UpdateAsync(token);
        }
    }
}
