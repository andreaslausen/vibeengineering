
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.Repositories;
using Zeiterfassung.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Zeiterfassung.Infrastructure.Repositories
{
    public class TimeEntryRepository : ITimeEntryRepository
    {
        private readonly ZeiterfassungDbContext _context;
        public TimeEntryRepository(ZeiterfassungDbContext context) => _context = context;

        public async Task<TimeEntry?> GetActiveEntryForUserAsync(UserId userId)
        {
            return await _context.TimeEntries.FirstOrDefaultAsync(e => e.UserId == userId && e.End == null && e.DeletedAt == null);
        }

        public async Task<List<TimeEntry>> GetEntriesForUserAsync(UserId userId)
        {
            return await _context.TimeEntries.Where(e => e.UserId == userId && e.DeletedAt == null).ToListAsync();
        }

        public async Task AddAsync(TimeEntry entry)
        {
            await _context.TimeEntries.AddAsync(entry);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TimeEntry entry)
        {
            _context.TimeEntries.Update(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasOverlapAsync(UserId userId, DateTime start, DateTime end, TimeEntryId? excludeId = null)
        {
            return await _context.TimeEntries.AnyAsync(e =>
                e.UserId == userId &&
                e.DeletedAt == null &&
                (excludeId == null || e.Id != excludeId) &&
                ((e.Start < end) && (e.End == null || e.End > start))
            );
        }
    }
}
