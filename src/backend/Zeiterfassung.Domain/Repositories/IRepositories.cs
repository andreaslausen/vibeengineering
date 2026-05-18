
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.ValueObjects;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Zeiterfassung.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(UserId id);
        Task<User?> GetByUsernameAsync(Username username);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }

    public interface ITimeEntryRepository
    {
        Task<TimeEntry?> GetActiveEntryForUserAsync(UserId userId);
        Task<List<TimeEntry>> GetEntriesForUserAsync(UserId userId);
        Task AddAsync(TimeEntry entry);
        Task UpdateAsync(TimeEntry entry);
        Task<bool> HasOverlapAsync(UserId userId, DateTime start, DateTime end, TimeEntryId? excludeId = null);
    }

    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByIdAsync(RefreshTokenId id);
        Task AddAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
    }
}
