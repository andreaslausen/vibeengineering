namespace Zeiterfassung.Domain
{
    public class User
    {
        public required ValueObjects.UserId Id { get; set; }
        public ValueObjects.Username Username { get; set; } = null!;
        public ValueObjects.PasswordHash PasswordHash { get; set; } = null!;
        public ValueObjects.Email? Email { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
