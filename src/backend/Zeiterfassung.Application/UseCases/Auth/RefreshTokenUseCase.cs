using Zeiterfassung.Application.Dtos.Auth;
using Zeiterfassung.Application.Services;
using Zeiterfassung.Domain.Repositories;
using Zeiterfassung.Domain.ValueObjects;

namespace Zeiterfassung.Application.UseCases.Auth
{
    /// <summary>
    /// Use Case für Token-Aktualisierung (Refresh Token Rotation mit Reuse Detection)
    /// </summary>
    public class RefreshTokenUseCase
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenService _jwtTokenService;

        public RefreshTokenUseCase(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            JwtTokenService jwtTokenService)
        {
            _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        }

        public async Task<RefreshTokenResponseDto> ExecuteAsync(RefreshTokenRequestDto request)
        {
            // Finde den Refresh Token
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (refreshToken == null)
                throw new InvalidOperationException("Ungültiger Refresh Token.");

            // Prüfe ob Token abgelaufen ist
            if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
                throw new InvalidOperationException("Refresh Token ist abgelaufen.");

            // Prüfe ob Token widerrufen wurde
            if (refreshToken.RevokedAt.HasValue)
                throw new InvalidOperationException("Refresh Token wurde widerrufen. Möglicher Missbrauch erkannt (Reuse Detection).");

            // Prüfe ob Token bereits durch einen neuen ersetzt wurde
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
                throw new InvalidOperationException("Refresh Token wurde bereits verwendet (Token Rotation). Neuer Token erforderlich.");

            // Generiere neuen Refresh Token (Token Rotation)
            var (newRefreshTokenString, newRefreshTokenExpiresAt) = _jwtTokenService.GenerateRefreshToken();

            // Markiere alten Token als ersetzt
            refreshToken.ReplacedByToken = newRefreshTokenString;
            await _refreshTokenRepository.UpdateAsync(refreshToken);

            // Speichere neuen Refresh Token
            var newRefreshToken = new Domain.RefreshToken
            {
                Id = new RefreshTokenId(Guid.NewGuid()),
                UserId = refreshToken.UserId,
                Token = newRefreshTokenString,
                ExpiresAt = newRefreshTokenExpiresAt,
                CreatedAt = DateTimeOffset.UtcNow,
                RevokedAt = null,
                ReplacedByToken = null
            };

            var user = await _userRepository.GetByIdAsync(refreshToken.UserId)
                ?? throw new InvalidOperationException("Benutzer für Refresh Token nicht gefunden.");

            var (newAccessToken, accessTokenExpiresAt) = _jwtTokenService.GenerateAccessToken(
                user.Id.Value.ToString(),
                user.Username.Value,
                newRefreshToken.Id.Value.ToString());

            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenString,
                AccessTokenExpiresAt = accessTokenExpiresAt
            };
        }
    }
}
