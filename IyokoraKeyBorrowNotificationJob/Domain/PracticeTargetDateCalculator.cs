namespace IyokoraKeyBorrowNotificationJob.Domain;

public static class PracticeTargetDateCalculator
{
    // 日本には夏時間がなく常に UTC+9 のため、TimeZoneInfo を使わず固定オフセットで変換する。
    private static readonly TimeSpan JstOffset = TimeSpan.FromHours(9);

    /// <summary>
    /// 基準日時(UTC)を JST のカレンダー日付に変換したうえで daysBefore 日後を求め、
    /// 時刻を持たない UTC 日付として返す。
    /// Firestore の practices.date は「日付のみを表す値」を UTC 0時の Timestamp として
    /// 保存しているため、検索キーもこの形に揃える。
    /// 例えば JST 08:00 に実行される場合、UTC 上ではまだ前日の 23:00 であり、
    /// UTC の日付をそのまま使うと対象日が1日ずれてしまう。
    /// </summary>
    public static DateTime GetTargetDate(DateTime utcNow, int daysBefore)
    {
        var jstDate = (utcNow + JstOffset).Date;
        return DateTime.SpecifyKind(jstDate.AddDays(daysBefore), DateTimeKind.Utc);
    }
}
