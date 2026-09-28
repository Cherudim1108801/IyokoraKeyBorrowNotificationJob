using Google.Cloud.Firestore;
using IyokoraKeyBorrowNotificationJob.Application;
using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Infrastructure;

public sealed class FirestorePracticeScheduleRepository(FirestoreDb firestoreDb) : IPracticeScheduleRepository
{
    public async Task<IReadOnlyList<PracticeSchedule>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        Query query = firestoreDb.Collection(PracticeScheduleOptions.CollectionName)
            .WhereEqualTo("groupId", PracticeScheduleOptions.GroupId)
            .WhereGreaterThanOrEqualTo("date", Timestamp.FromDateTime(start))
            .WhereLessThanOrEqualTo("date", Timestamp.FromDateTime(end));

        QuerySnapshot snapshot = await query.GetSnapshotAsync(cancellationToken);

        return snapshot.Documents.Select(doc => ToPracticeSchedule(doc.Id, doc.ToDictionary())).ToList();
    }

    /// <summary>
    /// Firestoreドキュメントのフィールド群から PracticeSchedule を組み立てる。
    /// DocumentSnapshot はテストコードから直接生成できないため、ToDictionary() で
    /// 取り出した素のフィールド値を受け取る形にして、フィールド名・型の対応関係を
    /// 単体テストで検証できるようにしている。
    /// </summary>
    public static PracticeSchedule ToPracticeSchedule(string id, IReadOnlyDictionary<string, object> data) => new(
        Id: id,
        Date: ((Timestamp)data["date"]).ToDateTime(),
        RequiresKeyPickup: data.TryGetValue("requiresKeyPickup", out var requiresKeyPickup) && (bool)requiresKeyPickup,
        KeyPickedUp: data.TryGetValue("keyPickedUp", out var keyPickedUp) && (bool)keyPickedUp);
}
