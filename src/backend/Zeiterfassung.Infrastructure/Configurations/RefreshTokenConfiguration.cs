using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.ValueObjects;

namespace Zeiterfassung.Infrastructure.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasConversion(id => id.Value, value => new RefreshTokenId(value));
            builder.Property(t => t.UserId)
                .HasConversion(id => id.Value, value => new UserId(value));
            builder.Property(t => t.Token).IsRequired();
            builder.Property(t => t.ExpiresAt).IsRequired();
            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.RevokedAt);
            builder.Property(t => t.ReplacedByToken);
            builder.HasIndex(t => t.Token).IsUnique();
        }
    }
}
