namespace IyokoraKeyBorrowNotificationJob.Application;

public sealed record SendKeyPickupRemindersResult(DateTime? NearestPracticeDate, int SchedulesInRange, int RemindersSent);
