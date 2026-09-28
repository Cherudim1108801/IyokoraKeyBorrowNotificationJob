namespace IyokoraKeyBorrowNotificationJob.Domain;

public static class PracticeReminderEvaluator
{
    /// <summary>
    /// 鍵の受け取りが必要な練習で、まだ受け取りが完了していない場合にリマインドが必要と判定する。
    /// </summary>
    public static bool NeedsReminder(bool requiresKeyPickup, bool keyPickedUp)
        => requiresKeyPickup && !keyPickedUp;
}
