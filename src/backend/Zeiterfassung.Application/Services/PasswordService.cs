namespace Zeiterfassung.Application.Services
{
    /// <summary>
    /// Service für Passwortvalidierung und -hashing
    /// </summary>
    public class PasswordService
    {
        /// <summary>
        /// Validiert ein Passwort gegen Regeln:
        /// - Mindestens 12 Zeichen
        /// - Wird gegen Blocklist kompromittierter Passwörter geprüft (vereinfacht)
        /// </summary>
        public bool ValidatePassword(string password, out string? errorMessage)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Passwort darf nicht leer sein.";
                return false;
            }

            if (password.Length < 12)
            {
                errorMessage = "Passwort muss mindestens 12 Zeichen lang sein.";
                return false;
            }

            // Vereinfachte Blocklist für häufig kompromittierte Passwörter
            var commonPasswords = new[] { "password123", "12345678901", "qwerty12345", "abc123456789" };
            if (commonPasswords.Any(cp => password.Equals(cp, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "Dieses Passwort ist zu häufig und nicht sicher genug.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Hasht ein Passwort mit BCrypt
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Vergleicht ein Plain-Text-Passwort mit einem gehashten Passwort
        /// </summary>
        public bool VerifyPassword(string plainPassword, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
        }
    }
}
