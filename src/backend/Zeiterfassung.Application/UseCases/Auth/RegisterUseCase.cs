using Zeiterfassung.Application.Dtos.Auth;
using Zeiterfassung.Application.Services;
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.Repositories;
using Zeiterfassung.Domain.ValueObjects;

namespace Zeiterfassung.Application.UseCases.Auth
{
    /// <summary>
    /// Use Case für Benutzerregistrierung
    /// </summary>
    public class RegisterUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;

        public RegisterUseCase(IUserRepository userRepository, PasswordService passwordService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        public async Task<RegisterResponseDto> ExecuteAsync(RegisterRequestDto request)
        {
            // Eingabevalidierung
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new InvalidOperationException("Benutzername ist erforderlich.");

            if (!_passwordService.ValidatePassword(request.Password, out var passwordError))
                throw new InvalidOperationException(passwordError);

            // Prüfe auf Duplikate
            var existingUser = await _userRepository.GetByUsernameAsync(new Username(request.Username));
            if (existingUser != null)
                throw new InvalidOperationException($"Benutzer mit dem Namen '{request.Username}' existiert bereits.");

            // Erstelle neuen Benutzer
            var user = new User
            {
                Id = new UserId(Guid.NewGuid()),
                Username = new Username(request.Username),
                PasswordHash = new PasswordHash(_passwordService.HashPassword(request.Password)),
                Email = !string.IsNullOrWhiteSpace(request.Email) ? new Email(request.Email) : null,
                CreatedAt = DateTimeOffset.UtcNow,
                DeletedAt = null
            };

            // Speichere Benutzer
            await _userRepository.AddAsync(user);

            return new RegisterResponseDto
            {
                UserId = user.Id.Value.ToString(),
                Username = user.Username.Value,
                Email = user.Email?.Value,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
