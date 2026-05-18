namespace Zeiterfassung.Domain.ValueObjects
{
    public sealed record Username
    {
        public string Value { get; }
        public Username(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Username darf nicht leer sein.", nameof(value));
            Value = value;
        }
        public override string ToString() => Value;
    }
}
