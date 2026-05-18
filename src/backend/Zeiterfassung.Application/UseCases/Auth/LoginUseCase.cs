using Zeiterfassung.Application.Dtos.Auth;
using Zeiterfassung.Application.Services;
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.Repositories;
using Zeiterfassung.Domain.ValueObjects;

namespace Zeiterfassung.Application.UseCases.Auth
{
    /// <summary>
    /// Use Case für Login
    /// </summary>
    public class LoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly PasswordService _passwordService;
        private readonly JwtTokenService _jwtTokenService;

        public LoginUseCase(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            PasswordService passwordService,
            JwtTokenService jwtTokenService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        }

        public async Task<LoginResponseDto> ExecuteAsync(LoginRequestDto request)
        {
            // Finde Benutzer nach Benutzername
            var user = await _userRepository.GetByUsernameAsync(new Username(request.Username));
            if (user == null)
                throw new InvalidOperationException("Ungültige Anmeldeinformationen.");

            // Verifiziere Passwort
            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash.Value))
                throw new InvalidOperationException("Ungültige Anmeldeinformationen.");

            // Generiere Refresh Token
            var (refreshTokenString, refreshTokenExpiresAt) = _jwtTokenService.GenerateRefreshToken();

            // Speichere Refresh Token in DB
            var refreshToken = new RefreshToken
            {
                Id = new RefreshTokenId(Guid.NewGuid()),
                UserId = user.Id,
                Token = refreshTokenString,
                ExpiresAt = refreshTokenExpiresAt,
                CreatedAt = DateTimeOffset.UtcNow,
                RevokedAt = null,
                ReplacedByToken = null
            };

            var (accessToken, accessTokenExpiresAt) = _jwtTokenService.GenerateAccessToken(
                user.Id.Value.ToString(),
                user.Username.Value,
                refreshToken.Id.Value.ToString());

            await _refreshTokenRepository.AddAsync(refreshToken);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                Username = user.Username.Value,
                UserId = user.Id.Value.ToString()
            };
        }
    }
}
