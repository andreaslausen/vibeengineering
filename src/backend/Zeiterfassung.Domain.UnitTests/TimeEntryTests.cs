using System;
using Zeiterfassung.Domain;
using Zeiterfassung.Domain.ValueObjects;
using Xunit;

namespace Zeiterfassung.Domain.UnitTests;

public class TimeEntryTests
{
    [Fact]
    public void StartNew_ShouldCreateActiveEntry_WithExpectedValues()
    {
        var id = new TimeEntryId(Guid.NewGuid());
        var userId = new UserId(Guid.NewGuid());
        var start = Utc(2026, 5, 18, 9, 0, 0);
        var createdAt = Utc(2026, 5, 18, 9, 1, 0);
        var serverNow = Utc(2026, 5, 18, 9, 2, 0);
        var note = new Note("Daily standup");
        var category = new Category("Meeting");

        var entry = TimeEntry.StartNew(id, userId, start, createdAt, serverNow, note, category);

        Assert.Equal(id, entry.Id);
        Assert.Equal(userId, entry.UserId);
        Assert.Equal(start, entry.Start);
        Assert.Equal(createdAt, entry.CreatedAt);
        Assert.Equal(note, entry.Note);
        Assert.Equal(category, entry.Category);
        Assert.True(entry.IsActive);
        Assert.Null(entry.End);
        Assert.Null(entry.Duration);
    }

    [Fact]
    public void StartNew_ShouldThrow_WhenStartIsInFuture()
    {
        var start = Utc(2026, 5, 18, 10, 0, 0);
        var serverNow = Utc(2026, 5, 18, 9, 59, 59);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TimeEntry.StartNew(new TimeEntryId(Guid.NewGuid()), new UserId(Guid.NewGuid()), start, start, serverNow));
    }

    [Fact]
    public void StartNew_ShouldThrow_WhenCreatedAtIsBeforeStart()
    {
        var start = Utc(2026, 5, 18, 10, 0, 0);
        var createdAt = Utc(2026, 5, 18, 9, 59, 0);
        var serverNow = Utc(2026, 5, 18, 10, 0, 0);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TimeEntry.StartNew(new TimeEntryId(Guid.NewGuid()), new UserId(Guid.NewGuid()), start, createdAt, serverNow));
    }

    [Fact]
    public void StartNew_ShouldThrow_WhenStartIsNotUtc()
    {
        var nonUtcStart = new DateTimeOffset(2026, 5, 18, 9, 0, 0, TimeSpan.FromHours(2));
        var createdAt = Utc(2026, 5, 18, 9, 1, 0);
        var serverNow = Utc(2026, 5, 18, 9, 2, 0);

        Assert.Throws<ArgumentException>(() =>
            TimeEntry.StartNew(new TimeEntryId(Guid.NewGuid()), new UserId(Guid.NewGuid()), nonUtcStart, createdAt, serverNow));
    }

    [Fact]
    public void Stop_ShouldSetEnd_AndDuration()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        var end = Utc(2026, 5, 18, 10, 15, 0);
        var serverNow = Utc(2026, 5, 18, 10, 30, 0);

        entry.Stop(end, serverNow);

        Assert.Equal(end, entry.End);
        Assert.Equal(TimeSpan.FromMinutes(75), entry.Duration);
        Assert.False(entry.IsActive);
    }

    [Fact]
    public void Stop_ShouldThrow_WhenEntryAlreadyStopped()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        entry.Stop(Utc(2026, 5, 18, 9, 30, 0), Utc(2026, 5, 18, 9, 31, 0));

        Assert.Throws<InvalidOperationException>(() =>
            entry.Stop(Utc(2026, 5, 18, 10, 0, 0), Utc(2026, 5, 18, 10, 1, 0)));
    }

    [Fact]
    public void Stop_ShouldThrow_WhenEndIsBeforeStart()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            entry.Stop(Utc(2026, 5, 18, 8, 59, 0), Utc(2026, 5, 18, 9, 0, 0)));
    }

    [Fact]
    public void Stop_ShouldThrow_WhenEndIsInFuture()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            entry.Stop(Utc(2026, 5, 18, 10, 0, 1), Utc(2026, 5, 18, 10, 0, 0)));
    }

    [Fact]
    public void Stop_ShouldThrow_WhenEndIsNotUtc()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        var nonUtcEnd = new DateTimeOffset(2026, 5, 18, 10, 0, 0, TimeSpan.FromHours(1));

        Assert.Throws<ArgumentException>(() =>
            entry.Stop(nonUtcEnd, Utc(2026, 5, 18, 10, 0, 0)));
    }

    [Fact]
    public void UpdateTimeRange_ShouldUpdateStartAndEnd()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        var newStart = Utc(2026, 5, 18, 8, 30, 0);
        var newEnd = Utc(2026, 5, 18, 9, 45, 0);
        var serverNow = Utc(2026, 5, 18, 10, 0, 0);

        entry.UpdateTimeRange(newStart, newEnd, serverNow);

        Assert.Equal(newStart, entry.Start);
        Assert.Equal(newEnd, entry.End);
        Assert.Equal(TimeSpan.FromMinutes(75), entry.Duration);
    }

    [Fact]
    public void UpdateTimeRange_ShouldAllowOpenEntry_WhenEndIsNull()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        var newStart = Utc(2026, 5, 18, 8, 45, 0);
        var serverNow = Utc(2026, 5, 18, 9, 0, 0);

        entry.UpdateTimeRange(newStart, null, serverNow);

        Assert.Equal(newStart, entry.Start);
        Assert.Null(entry.End);
        Assert.Null(entry.Duration);
    }

    [Fact]
    public void UpdateTimeRange_ShouldThrow_WhenStartIsInFuture()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            entry.UpdateTimeRange(Utc(2026, 5, 18, 10, 0, 1), null, Utc(2026, 5, 18, 10, 0, 0)));
    }

    [Fact]
    public void UpdateTimeRange_ShouldThrow_WhenEndBeforeStart()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            entry.UpdateTimeRange(Utc(2026, 5, 18, 9, 0, 0), Utc(2026, 5, 18, 8, 59, 59), Utc(2026, 5, 18, 9, 0, 0)));
    }

    [Fact]
    public void UpdateTimeRange_ShouldThrow_WhenEndIsInFuture()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            entry.UpdateTimeRange(Utc(2026, 5, 18, 9, 0, 0), Utc(2026, 5, 18, 10, 0, 1), Utc(2026, 5, 18, 10, 0, 0)));
    }

    [Fact]
    public void UpdateTimeRange_ShouldThrow_WhenEndIsNotUtc()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        var nonUtcEnd = new DateTimeOffset(2026, 5, 18, 10, 0, 0, TimeSpan.FromHours(2));

        Assert.Throws<ArgumentException>(() =>
            entry.UpdateTimeRange(Utc(2026, 5, 18, 9, 0, 0), nonUtcEnd, Utc(2026, 5, 18, 10, 0, 0)));
    }

    [Fact]
    public void UpdateMetadata_ShouldSetNoteAndCategory()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        var note = new Note("Refactoring");
        var category = new Category("Coding");

        entry.UpdateMetadata(note, category);

        Assert.Equal(note, entry.Note);
        Assert.Equal(category, entry.Category);
    }

    [Fact]
    public void SoftDelete_ShouldSetDeletedAt_AndDeactivateEntry()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        var deletedAt = Utc(2026, 5, 18, 11, 0, 0);

        entry.SoftDelete(deletedAt);

        Assert.Equal(deletedAt, entry.DeletedAt);
        Assert.False(entry.IsActive);
    }

    [Fact]
    public void SoftDelete_ShouldThrow_WhenAlreadyDeleted()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        entry.SoftDelete(Utc(2026, 5, 18, 10, 0, 0));

        Assert.Throws<InvalidOperationException>(() =>
            entry.SoftDelete(Utc(2026, 5, 18, 10, 5, 0)));
    }

    [Fact]
    public void SoftDelete_ShouldThrow_WhenTimestampIsNotUtc()
    {
        var entry = CreateActiveEntry(Utc(2026, 5, 18, 9, 0, 0));
        var nonUtcDeletedAt = new DateTimeOffset(2026, 5, 18, 10, 0, 0, TimeSpan.FromHours(1));

        Assert.Throws<ArgumentException>(() => entry.SoftDelete(nonUtcDeletedAt));
    }

    private static TimeEntry CreateActiveEntry(DateTimeOffset startUtc)
    {
        return TimeEntry.StartNew(
            new TimeEntryId(Guid.NewGuid()),
            new UserId(Guid.NewGuid()),
            startUtc,
            startUtc,
            startUtc);
    }

    private static DateTimeOffset Utc(int year, int month, int day, int hour, int minute, int second)
    {
        return new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero);
    }
}