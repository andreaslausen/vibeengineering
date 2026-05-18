namespace Zeiterfassung.Domain
{
    public class RefreshToken
    {
        public required ValueObjects.RefreshTokenId Id { get; set; }
        public required ValueObjects.UserId UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
    }
}
