namespace Zeiterfassung.Domain
{
    public class TimeEntry
    {
        public ValueObjects.TimeEntryId Id { get; private set; }
        public ValueObjects.UserId UserId { get; private set; }
        public DateTimeOffset Start { get; private set; }
        public DateTimeOffset? End { get; private set; }
        public ValueObjects.Note? Note { get; private set; }
        public ValueObjects.Category? Category { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? DeletedAt { get; private set; }

        public bool IsActive => End is null && DeletedAt is null;
        public TimeSpan? Duration => End is null ? null : End.Value - Start;

        private TimeEntry(
            ValueObjects.TimeEntryId id,
            ValueObjects.UserId userId,
            DateTimeOffset start,
            DateTimeOffset createdAt,
            ValueObjects.Note? note,
            ValueObjects.Category? category)
        {
            Id = id;
            UserId = userId;
            Start = start;
            CreatedAt = createdAt;
            Note = note;
            Category = category;
        }

        public static TimeEntry StartNew(
            ValueObjects.TimeEntryId id,
            ValueObjects.UserId userId,
            DateTimeOffset startUtc,
            DateTimeOffset createdAtUtc,
            DateTimeOffset serverNowUtc,
            ValueObjects.Note? note = null,
            ValueObjects.Category? category = null)
        {
            EnsureUtc(startUtc, nameof(startUtc));
            EnsureUtc(createdAtUtc, nameof(createdAtUtc));
            EnsureUtc(serverNowUtc, nameof(serverNowUtc));

            if (startUtc > serverNowUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(startUtc), "Start darf nicht in der Zukunft liegen.");
            }

            if (createdAtUtc < startUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(createdAtUtc), "CreatedAt darf nicht vor Start liegen.");
            }

            return new TimeEntry(id, userId, startUtc, createdAtUtc, note, category);
        }

        public void Stop(DateTimeOffset endUtc, DateTimeOffset serverNowUtc)
        {
            EnsureUtc(endUtc, nameof(endUtc));
            EnsureUtc(serverNowUtc, nameof(serverNowUtc));

            if (!IsActive)
            {
                throw new InvalidOperationException("Ein bereits beendeter oder gelöschter Eintrag kann nicht gestoppt werden.");
            }

            if (endUtc < Start)
            {
                throw new ArgumentOutOfRangeException(nameof(endUtc), "Ende muss nach oder gleich Start liegen.");
            }

            if (endUtc > serverNowUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(endUtc), "Ende darf nicht in der Zukunft liegen.");
            }

            End = endUtc;
        }

        public void UpdateTimeRange(DateTimeOffset newStartUtc, DateTimeOffset? newEndUtc, DateTimeOffset serverNowUtc)
        {
            EnsureUtc(newStartUtc, nameof(newStartUtc));
            EnsureUtc(serverNowUtc, nameof(serverNowUtc));

            if (newEndUtc.HasValue)
            {
                EnsureUtc(newEndUtc.Value, nameof(newEndUtc));
            }

            if (newStartUtc > serverNowUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(newStartUtc), "Start darf nicht in der Zukunft liegen.");
            }

            if (newEndUtc.HasValue && newEndUtc.Value < newStartUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(newEndUtc), "Ende muss nach oder gleich Start liegen.");
            }

            if (newEndUtc.HasValue && newEndUtc.Value > serverNowUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(newEndUtc), "Ende darf nicht in der Zukunft liegen.");
            }

            Start = newStartUtc;
            End = newEndUtc;
        }

        public void UpdateMetadata(ValueObjects.Note? note, ValueObjects.Category? category)
        {
            Note = note;
            Category = category;
        }

        public void SoftDelete(DateTimeOffset deletedAtUtc)
        {
            EnsureUtc(deletedAtUtc, nameof(deletedAtUtc));

            if (DeletedAt is not null)
            {
                throw new InvalidOperationException("Der Eintrag ist bereits gelöscht.");
            }

            DeletedAt = deletedAtUtc;
        }

        private static void EnsureUtc(DateTimeOffset value, string paramName)
        {
            if (value.Offset != TimeSpan.Zero)
            {
                throw new ArgumentException("Zeitstempel muss UTC sein (Offset 00:00).", paramName);
            }
        }
    }
}
