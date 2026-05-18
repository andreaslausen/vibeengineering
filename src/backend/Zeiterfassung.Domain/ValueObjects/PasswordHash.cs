namespace Zeiterfassung.Domain.ValueObjects
{
    public sealed record PasswordHash
    {
        public string Value { get; }
        public PasswordHash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("PasswordHash darf nicht leer sein.", nameof(value));
            Value = value;
        }
        public override string ToString() => Value;
    }
}
