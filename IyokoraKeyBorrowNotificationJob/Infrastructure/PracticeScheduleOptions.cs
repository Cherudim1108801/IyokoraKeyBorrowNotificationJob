namespace IyokoraKeyBorrowNotificationJob.Infrastructure;

/// <summary>
/// Firestore の practices コレクションに関する定数。
/// IyokoraAttendanceWebAssembly / IyokoraAttendanceApp の FirebaseOptions.GroupId と値を揃えること。
/// </summary>
public static class PracticeScheduleOptions
{
    public const string CollectionName = "practices";

    public const string GroupId = "default";
}
