namespace IyokoraKeyBorrowNotificationJob;

public static class ReminderMessageBuilder
{
    public static string Build(int daysBefore, DateTime practiceDate)
        => $"【リマインド】{daysBefore}日後の練習({practiceDate:yyyy/MM/dd})の鍵の受け取りが未登録です。";
}
