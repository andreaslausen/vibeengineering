using Microsoft.EntityFrameworkCore;
using Zeiterfassung.Domain;

namespace Zeiterfassung.Infrastructure
{
    public class ZeiterfassungDbContext : DbContext
    {
        public ZeiterfassungDbContext(DbContextOptions<ZeiterfassungDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TimeEntryConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RefreshTokenConfiguration());

            // Soft-Delete-Filter für TimeEntries
            modelBuilder.Entity<TimeEntry>().HasQueryFilter(e => e.DeletedAt == null);

            base.OnModelCreating(modelBuilder);
        }
    }
}
