
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.Repositories;
using Zeiterfassung.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Zeiterfassung.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ZeiterfassungDbContext _context;
        public RefreshTokenRepository(ZeiterfassungDbContext context) => _context = context;

        public async Task<RefreshToken?> GetByIdAsync(RefreshTokenId id)
        {
            return await _context.RefreshTokens.FindAsync(id);
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
