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
Console.WriteLine($"接続先Firestoreプロジェクト: {projectId}");
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

const int lookAheadDays = 7;
var result = await useCase.ExecuteAsync(DateTime.UtcNow, lookAheadDays);

Console.WriteLine(
    result.NearestPracticeDate is { } nearestPracticeDate
        ? $"直近の対象日(JST基準): {nearestPracticeDate:yyyy-MM-dd} / {lookAheadDays}日以内の練習件数: {result.SchedulesInRange} / 送信件数: {result.RemindersSent}"
        : $"{lookAheadDays}日以内に該当する練習が見つかりませんでした。");
