using Zeiterfassung.Application.Dtos.Auth;
using Zeiterfassung.Domain.Repositories;
using Zeiterfassung.Domain.ValueObjects;

namespace Zeiterfassung.Application.UseCases.Auth
{
    /// <summary>
    /// Use Case für Profil-Abruf (/auth/me)
    /// </summary>
    public class GetUserProfileUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserProfileUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<UserProfileDto> ExecuteAsync(string userId)
        {
            // Finde Benutzer
            var user = await _userRepository.GetByIdAsync(new UserId(Guid.Parse(userId)));
            if (user == null)
                throw new InvalidOperationException("Benutzer nicht gefunden.");

            return new UserProfileDto
            {
                UserId = user.Id.Value.ToString(),
                Username = user.Username.Value,
                Email = user.Email?.Value,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
