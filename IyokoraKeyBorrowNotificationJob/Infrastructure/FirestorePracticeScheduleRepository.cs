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

        return snapshot.Documents.Select(ToPracticeSchedule).ToList();
    }

    private static PracticeSchedule ToPracticeSchedule(DocumentSnapshot doc) => new(
        Id: doc.Id,
        Date: doc.GetValue<Timestamp>("date").ToDateTime(),
        RequiresKeyPickup: doc.ContainsField("requiresKeyPickup") && doc.GetValue<bool>("requiresKeyPickup"),
        KeyPickedUp: doc.ContainsField("keyPickedUp") && doc.GetValue<bool>("keyPickedUp"));
}
