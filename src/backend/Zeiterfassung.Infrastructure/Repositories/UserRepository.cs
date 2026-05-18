
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.Repositories;
using Zeiterfassung.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Zeiterfassung.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ZeiterfassungDbContext _context;
        public UserRepository(ZeiterfassungDbContext context) => _context = context;

        public async Task<User?> GetByIdAsync(UserId id) =>
            await _context.Users.FindAsync(id);

        public async Task<User?> GetByUsernameAsync(Username username) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
