namespace Zeiterfassung.Domain.ValueObjects
{
    public sealed record Note
    {
        public string? Value { get; }
        public Note(string? value)
        {
            Value = value;
        }
        public override string ToString() => Value ?? string.Empty;
    }
}
