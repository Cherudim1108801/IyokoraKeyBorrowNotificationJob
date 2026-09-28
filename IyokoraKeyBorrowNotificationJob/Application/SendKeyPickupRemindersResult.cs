namespace IyokoraKeyBorrowNotificationJob.Application;

public sealed record SendKeyPickupRemindersResult(DateTime TargetDate, int SchedulesFound, int RemindersSent);
