namespace Zeiterfassung.Domain
{
    public static class TimeEntryOverlapPolicy
    {
        public static bool HasOverlap(
            IEnumerable<TimeEntry> existingEntries,
            ValueObjects.UserId userId,
            DateTimeOffset candidateStartUtc,
            DateTimeOffset? candidateEndUtc,
            DateTimeOffset serverNowUtc,
            ValueObjects.TimeEntryId? ignoreTimeEntryId = null)
        {
            EnsureUtc(candidateStartUtc, nameof(candidateStartUtc));
            EnsureUtc(serverNowUtc, nameof(serverNowUtc));

            if (candidateEndUtc.HasValue)
            {
                EnsureUtc(candidateEndUtc.Value, nameof(candidateEndUtc));
            }

            if (candidateStartUtc > serverNowUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(candidateStartUtc), "Start darf nicht in der Zukunft liegen.");
            }

            if (candidateEndUtc.HasValue && candidateEndUtc.Value < candidateStartUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(candidateEndUtc), "Ende muss nach oder gleich Start liegen.");
            }

            if (candidateEndUtc.HasValue && candidateEndUtc.Value > serverNowUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(candidateEndUtc), "Ende darf nicht in der Zukunft liegen.");
            }

            var candidateEffectiveEndUtc = candidateEndUtc ?? serverNowUtc;

            foreach (var existing in existingEntries)
            {
                if (existing.UserId != userId)
                {
                    continue;
                }

                if (existing.DeletedAt is not null)
                {
                    continue;
                }

                if (ignoreTimeEntryId is not null && existing.Id == ignoreTimeEntryId)
                {
                    continue;
                }

                var existingEffectiveEndUtc = existing.End ?? serverNowUtc;

                // Closed-open intervals: [start, end)
                if (candidateStartUtc < existingEffectiveEndUtc && existing.Start < candidateEffectiveEndUtc)
                {
                    return true;
                }
            }

            return false;
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
