namespace IyokoraKeyBorrowNotificationJob.Domain;

public static class PracticeSearchRangeCalculator
{
    // 日本には夏時間がなく常に UTC+9 のため、TimeZoneInfo を使わず固定オフセットで変換する。
    private static readonly TimeSpan JstOffset = TimeSpan.FromHours(9);

    /// <summary>
    /// 基準日時(UTC)を JST のカレンダー日付に変換し、そこから lookAheadDays 日後までの
    /// 範囲(両端を含む)を、時刻を持たない UTC 日付として返す。
    /// Firestore の practices.date は「日付のみを表す値」を UTC 0時の Timestamp として
    /// 保存しているため、検索キーもこの形に揃える。
    /// </summary>
    public static (DateTime Start, DateTime End) GetRange(DateTime utcNow, int lookAheadDays)
    {
        var jstToday = DateTime.SpecifyKind((utcNow + JstOffset).Date, DateTimeKind.Utc);
        return (jstToday, jstToday.AddDays(lookAheadDays));
    }
}
