using Zeiterfassung.Domain.ValueObjects;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Zeiterfassung.Domain.Repositories
{
    public interface ITimeEntryRepository
    {
        Task<TimeEntry?> GetActiveEntryForUserAsync(UserId userId);
        Task<List<TimeEntry>> GetEntriesForUserAsync(UserId userId);
        Task AddAsync(TimeEntry entry);
        Task UpdateAsync(TimeEntry entry);
        Task<bool> HasOverlapAsync(UserId userId, DateTime start, DateTime end, TimeEntryId? excludeId = null);
    }
}
