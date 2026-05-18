namespace Zeiterfassung.Application.Dtos.Auth
{
    public class LoginResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTimeOffset AccessTokenExpiresAt { get; set; }
        public required string Username { get; set; }
        public required string UserId { get; set; }
    }
}
