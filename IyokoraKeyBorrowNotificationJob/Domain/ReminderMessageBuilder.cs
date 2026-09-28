namespace IyokoraKeyBorrowNotificationJob.Domain;

public static class ReminderMessageBuilder
{
    public static string Build(int daysUntil, DateTime practiceDate)
        => $"【リマインド】{daysUntil}日後の練習({practiceDate:yyyy/MM/dd})の鍵の受け取りが未登録です。";
}
