namespace IyokoraKeyBorrowNotificationJob.Domain;

public sealed record PracticeSchedule(string Id, DateTime Date, bool RequiresKeyPickup, bool KeyPickedUp)
{
    public bool NeedsReminder => PracticeReminderEvaluator.NeedsReminder(RequiresKeyPickup, KeyPickedUp);
}
