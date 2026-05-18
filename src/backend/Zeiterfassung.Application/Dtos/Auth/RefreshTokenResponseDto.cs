namespace Zeiterfassung.Application.Dtos.Auth
{
    public class RefreshTokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTimeOffset AccessTokenExpiresAt { get; set; }
    }
}
