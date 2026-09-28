using Google.Cloud.Firestore;
using System.Text;
using System.Text.Json;

var serviceAccountJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT")
    ?? throw new InvalidOperationException("FIREBASE_SERVICE_ACCOUNT が設定されていません");

// サービスアカウントJSONを一時ファイルに書き出し、認証に利用
var credPath = Path.Combine(Path.GetTempPath(), "firebase-sa.json");
await File.WriteAllTextAsync(credPath, serviceAccountJson);
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);

var projectId = JsonDocument.Parse(serviceAccountJson).RootElement.GetProperty("project_id").GetString();
FirestoreDb db = FirestoreDb.Create(projectId);

const int daysBefore = 3;
var targetDate = DateTime.UtcNow.Date.AddDays(daysBefore);
var nextDay = targetDate.AddDays(1);

Query query = db.Collection("practiceSchedules")
    .WhereGreaterThanOrEqualTo("practiceDate", Timestamp.FromDateTime(DateTime.SpecifyKind(targetDate, DateTimeKind.Utc)))
    .WhereLessThan("practiceDate", Timestamp.FromDateTime(DateTime.SpecifyKind(nextDay, DateTimeKind.Utc)));

QuerySnapshot snapshot = await query.GetSnapshotAsync();

foreach (var doc in snapshot.Documents)
{
    bool keyPickupRegistered = doc.ContainsField("keyPickupRegistered")
        && doc.GetValue<bool>("keyPickupRegistered");

    if (!keyPickupRegistered)
    {
        var practiceDate = doc.GetValue<Timestamp>("practiceDate").ToDateTime();
        await SendLineMessageAsync(
            $"【リマインド】{daysBefore}日後の練習({practiceDate:yyyy/MM/dd})の鍵の受け取りが未登録です。");
    }
}

async Task SendLineMessageAsync(string text)
{
    using var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", Environment.GetEnvironmentVariable("LINE_CHANNEL_ACCESS_TOKEN"));

    var payload = new
    {
        to = Environment.GetEnvironmentVariable("LINE_USER_ID"),
        messages = new[] { new { type = "text", text } }
    };

    var content = new StringContent(
        JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    var response = await client.PostAsync("https://api.line.me/v2/bot/message/push", content);
    response.EnsureSuccessStatusCode();
}