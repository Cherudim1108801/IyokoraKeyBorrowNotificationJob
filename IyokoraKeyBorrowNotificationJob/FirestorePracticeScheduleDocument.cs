using Google.Cloud.Firestore;

namespace IyokoraKeyBorrowNotificationJob;

/// <summary>practices コレクションのドキュメントから、リマインド判定に必要な項目だけを取り出すラッパー。</summary>
public sealed class FirestorePracticeScheduleDocument(DocumentSnapshot snapshot)
{
    public DateTime PracticeDate => snapshot.GetValue<Timestamp>("date").ToDateTime();

    public bool RequiresKeyPickup => snapshot.ContainsField("requiresKeyPickup")
        && snapshot.GetValue<bool>("requiresKeyPickup");

    public bool KeyPickedUp => snapshot.ContainsField("keyPickedUp")
        && snapshot.GetValue<bool>("keyPickedUp");
}
