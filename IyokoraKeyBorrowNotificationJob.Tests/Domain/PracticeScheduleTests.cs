using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Tests.Domain;

public class PracticeScheduleTests
{
    [Theory]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(false, false, false)]
    public void NeedsReminder_DelegatesToEvaluator(bool requiresKeyPickup, bool keyPickedUp, bool expected)
    {
        var schedule = new PracticeSchedule("p1", DateTime.UtcNow, requiresKeyPickup, keyPickedUp);

        Assert.Equal(expected, schedule.NeedsReminder);
    }
}
