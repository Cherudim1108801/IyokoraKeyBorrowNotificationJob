using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Tests.Domain;

public class PracticeSearchRangeCalculatorTests
{
    [Fact]
    public void GetRange_ReturnsTodayThroughLookAheadDaysLater()
    {
        var utcNow = new DateTime(2026, 4, 1, 13, 45, 30, DateTimeKind.Utc);

        var (start, end) = PracticeSearchRangeCalculator.GetRange(utcNow, lookAheadDays: 7);

        Assert.Equal(new DateTime(2026, 4, 1), start.Date);
        Assert.Equal(TimeSpan.Zero, start.TimeOfDay);
        Assert.Equal(DateTimeKind.Utc, start.Kind);
        Assert.Equal(new DateTime(2026, 4, 8), end.Date);
        Assert.Equal(DateTimeKind.Utc, end.Kind);
    }

    [Fact]
    public void GetRange_CrossesMonthBoundary()
    {
        var utcNow = new DateTime(2026, 4, 30, 0, 0, 0, DateTimeKind.Utc);

        var (start, end) = PracticeSearchRangeCalculator.GetRange(utcNow, lookAheadDays: 7);

        Assert.Equal(new DateTime(2026, 4, 30), start.Date);
        Assert.Equal(new DateTime(2026, 5, 7), end.Date);
    }

    [Fact]
    public void GetRange_UsesJstCalendarDate_WhenUtcIsStillPreviousDay()
    {
        // 2026-05-02 08:00 JST = 2026-05-01 23:00 UTC (UTC上ではまだ前日)
        var utcNow = new DateTime(2026, 5, 1, 23, 0, 0, DateTimeKind.Utc);

        var (start, end) = PracticeSearchRangeCalculator.GetRange(utcNow, lookAheadDays: 7);

        Assert.Equal(new DateTime(2026, 5, 2), start.Date);
        Assert.Equal(new DateTime(2026, 5, 9), end.Date);
    }
}
