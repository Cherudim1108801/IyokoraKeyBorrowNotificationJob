namespace IyokoraKeyBorrowNotificationJob.Tests;

public class PracticeReminderEvaluatorTests
{
    [Theory]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    public void NeedsReminder_ReturnsExpected(bool requiresKeyPickup, bool keyPickedUp, bool expected)
    {
        var actual = PracticeReminderEvaluator.NeedsReminder(requiresKeyPickup, keyPickedUp);

        Assert.Equal(expected, actual);
    }
}
