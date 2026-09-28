using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Tests.Domain;

public class PracticeTargetDateCalculatorTests
{
    [Fact]
    public void GetTargetDate_AddsDaysAndStripsTime()
    {
        var utcNow = new DateTime(2026, 4, 1, 13, 45, 30, DateTimeKind.Utc);

        var targetDate = PracticeTargetDateCalculator.GetTargetDate(utcNow, daysBefore: 3);

        Assert.Equal(new DateTime(2026, 4, 4), targetDate.Date);
        Assert.Equal(TimeSpan.Zero, targetDate.TimeOfDay);
        Assert.Equal(DateTimeKind.Utc, targetDate.Kind);
    }

    [Fact]
    public void GetTargetDate_CrossesMonthBoundary()
    {
        var utcNow = new DateTime(2026, 4, 30, 0, 0, 0, DateTimeKind.Utc);

        var targetDate = PracticeTargetDateCalculator.GetTargetDate(utcNow, daysBefore: 3);

        Assert.Equal(new DateTime(2026, 5, 3), targetDate.Date);
    }

    [Fact]
    public void GetTargetDate_UsesJstCalendarDate_WhenUtcIsStillPreviousDay()
    {
        // 2026-05-02 08:00 JST = 2026-05-01 23:00 UTC (UTC上ではまだ前日)
        var utcNow = new DateTime(2026, 5, 1, 23, 0, 0, DateTimeKind.Utc);

        var targetDate = PracticeTargetDateCalculator.GetTargetDate(utcNow, daysBefore: 3);

        // JST基準の当日(5/2)から3日後の5/5になるべきで、UTC基準の5/1から3日後の5/4にはならない
        Assert.Equal(new DateTime(2026, 5, 5), targetDate.Date);
    }
}
