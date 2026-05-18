namespace Zeiterfassung.Application.Dtos.Auth
{
    public class RegisterResponseDto
    {
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public string? Email { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
