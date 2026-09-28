namespace IyokoraKeyBorrowNotificationJob.Domain;

public static class PracticeTargetDateCalculator
{
    /// <summary>
    /// 基準日時から daysBefore 日後の、時刻を持たない UTC 日付を求める。
    /// Firestore の practices.date は日付のみを表す UTC 0時の Timestamp として保存されているため、
    /// 完全一致で検索できる形に揃える。
    /// </summary>
    public static DateTime GetTargetDate(DateTime utcNow, int daysBefore)
        => DateTime.SpecifyKind(utcNow.Date.AddDays(daysBefore), DateTimeKind.Utc);
}
