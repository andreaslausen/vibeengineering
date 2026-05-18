namespace Zeiterfassung.Application.Dtos.Auth
{
    public class LogoutRequestDto
    {
        public required string RefreshToken { get; set; }
    }
}
