using Zeiterfassung.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Zeiterfassung.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(UserId id);
        Task<User?> GetByUsernameAsync(Username username);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
