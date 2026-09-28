using Google.Cloud.Firestore;
using IyokoraKeyBorrowNotificationJob;

var serviceAccountJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT")
    ?? throw new InvalidOperationException("FIREBASE_SERVICE_ACCOUNT が設定されていません");

// サービスアカウントJSONを一時ファイルに書き出し、認証に利用
var credPath = Path.Combine(Path.GetTempPath(), "firebase-sa.json");
await File.WriteAllTextAsync(credPath, serviceAccountJson);
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);

var projectId = ServiceAccountCredentialHelper.GetProjectId(serviceAccountJson);
FirestoreDb db = FirestoreDb.Create(projectId);

const int daysBefore = 3;
var targetDate = PracticeTargetDateCalculator.GetTargetDate(DateTime.UtcNow, daysBefore);

Query query = db.Collection(PracticeScheduleOptions.CollectionName)
    .WhereEqualTo("groupId", PracticeScheduleOptions.GroupId)
    .WhereEqualTo("date", Timestamp.FromDateTime(targetDate));

QuerySnapshot snapshot = await query.GetSnapshotAsync();

using var httpClient = new HttpClient();
LineMessagingClient? lineClient = null;

foreach (var doc in snapshot.Documents)
{
    var practice = new FirestorePracticeScheduleDocument(doc);

    if (!PracticeReminderEvaluator.NeedsReminder(practice.RequiresKeyPickup, practice.KeyPickedUp))
    {
        continue;
    }

    lineClient ??= new LineMessagingClient(
        httpClient,
        Environment.GetEnvironmentVariable("LINE_CHANNEL_ACCESS_TOKEN")
            ?? throw new InvalidOperationException("LINE_CHANNEL_ACCESS_TOKEN が設定されていません"),
        Environment.GetEnvironmentVariable("LINE_USER_ID")
            ?? throw new InvalidOperationException("LINE_USER_ID が設定されていません"));

    await lineClient.SendMessageAsync(ReminderMessageBuilder.Build(daysBefore, practice.PracticeDate));
}
