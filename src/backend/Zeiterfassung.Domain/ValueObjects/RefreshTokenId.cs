namespace Zeiterfassung.Domain.ValueObjects
{
    public sealed record RefreshTokenId(Guid Value)
    {
        public override string ToString() => Value.ToString();
        public static implicit operator Guid(RefreshTokenId id) => id.Value;
        public static implicit operator RefreshTokenId(Guid value) => new(value);
    }
}
