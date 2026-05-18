namespace Zeiterfassung.Domain.ValueObjects
{
    public sealed record Category
    {
        public string? Value { get; }
        public Category(string? value)
        {
            Value = value;
        }
        public override string ToString() => Value ?? string.Empty;
    }
}
