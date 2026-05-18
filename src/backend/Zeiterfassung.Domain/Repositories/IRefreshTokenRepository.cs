using Zeiterfassung.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Zeiterfassung.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByIdAsync(RefreshTokenId id);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task AddAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
    }
}
