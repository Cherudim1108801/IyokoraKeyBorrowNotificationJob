using Google.Cloud.Firestore;
using IyokoraKeyBorrowNotificationJob.Application;
using IyokoraKeyBorrowNotificationJob.Infrastructure;

var serviceAccountJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT")
    ?? throw new InvalidOperationException("FIREBASE_SERVICE_ACCOUNT が設定されていません");

// サービスアカウントJSONを一時ファイルに書き出し、認証に利用
var credPath = Path.Combine(Path.GetTempPath(), "firebase-sa.json");
await File.WriteAllTextAsync(credPath, serviceAccountJson);
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);

var projectId = ServiceAccountCredentialHelper.GetProjectId(serviceAccountJson);
FirestoreDb firestoreDb = FirestoreDb.Create(projectId);

using var httpClient = new HttpClient();

IPracticeScheduleRepository practiceScheduleRepository = new FirestorePracticeScheduleRepository(firestoreDb);
INotificationSender notificationSender = new LineNotificationSender(
    httpClient,
    Environment.GetEnvironmentVariable("LINE_CHANNEL_ACCESS_TOKEN")
        ?? throw new InvalidOperationException("LINE_CHANNEL_ACCESS_TOKEN が設定されていません"),
    Environment.GetEnvironmentVariable("LINE_USER_ID")
        ?? throw new InvalidOperationException("LINE_USER_ID が設定されていません"));

var useCase = new SendKeyPickupRemindersUseCase(practiceScheduleRepository, notificationSender);

const int daysBefore = 3;
await useCase.ExecuteAsync(DateTime.UtcNow, daysBefore);
