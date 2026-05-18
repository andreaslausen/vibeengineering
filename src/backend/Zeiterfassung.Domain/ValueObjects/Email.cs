using System.Text.RegularExpressions;

namespace Zeiterfassung.Domain.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }
        public Email(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && !IsValidEmail(value))
                throw new ArgumentException($"Ungültige E-Mail-Adresse: {value}", nameof(value));
            Value = value;
        }
        public override string ToString() => Value;
        private static bool IsValidEmail(string email)
        {
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
    }
}
