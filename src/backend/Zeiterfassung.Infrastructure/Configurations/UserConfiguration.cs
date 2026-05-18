using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.ValueObjects;

namespace Zeiterfassung.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasConversion(id => id.Value, value => new UserId(value));
            builder.Property(u => u.Username)
                .HasConversion(u => u.Value, value => new Username(value))
                .IsRequired();
            builder.Property(u => u.PasswordHash)
                .HasConversion(p => p.Value, value => new PasswordHash(value))
                .IsRequired();
            builder.Property(u => u.Email)
                .HasConversion(e => e != null ? e.Value : null, value => value != null ? new Email(value) : null);
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.DeletedAt);
            builder.HasIndex(u => u.Username).IsUnique();
        }
    }
}
