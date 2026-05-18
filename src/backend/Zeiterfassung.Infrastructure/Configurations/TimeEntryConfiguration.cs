using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.ValueObjects;

namespace Zeiterfassung.Infrastructure.Configurations
{
    public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
    {
        public void Configure(EntityTypeBuilder<TimeEntry> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasConversion(id => id.Value, value => new TimeEntryId(value));
            builder.Property(e => e.UserId)
                .HasConversion(id => id.Value, value => new UserId(value));
            builder.Property(e => e.Start).IsRequired();
            builder.Property(e => e.End);
            builder.Property(e => e.Note)
                .HasConversion(n => n != null ? n.Value : null, value => value != null ? new Note(value) : null);
            builder.Property(e => e.Category)
                .HasConversion(c => c != null ? c.Value : null, value => value != null ? new Category(value) : null);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.DeletedAt);
            builder.HasIndex(e => new { e.UserId, e.Start });
            builder.HasIndex(e => e.DeletedAt);
        }
    }
}
