namespace Zeiterfassung.Domain.ValueObjects
{
    public sealed record UserId(Guid Value)
    {
        public override string ToString() => Value.ToString();
        public static implicit operator Guid(UserId id) => id.Value;
        public static implicit operator UserId(Guid value) => new(value);
    }
}
