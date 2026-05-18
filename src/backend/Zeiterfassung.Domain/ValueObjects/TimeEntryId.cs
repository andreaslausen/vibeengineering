namespace Zeiterfassung.Domain.ValueObjects
{
    public sealed record TimeEntryId(Guid Value)
    {
        public override string ToString() => Value.ToString();
        public static implicit operator Guid(TimeEntryId id) => id.Value;
        public static implicit operator TimeEntryId(Guid value) => new(value);
    }
}
