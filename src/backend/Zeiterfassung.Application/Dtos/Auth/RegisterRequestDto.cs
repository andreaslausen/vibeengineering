namespace Zeiterfassung.Application.Dtos.Auth
{
    public class RegisterRequestDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }
    }
}
