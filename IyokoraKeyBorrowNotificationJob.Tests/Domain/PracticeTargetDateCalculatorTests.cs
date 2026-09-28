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
}
