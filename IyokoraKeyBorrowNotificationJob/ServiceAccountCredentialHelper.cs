using System.Text.Json;

namespace IyokoraKeyBorrowNotificationJob;

public static class ServiceAccountCredentialHelper
{
    public static string GetProjectId(string serviceAccountJson)
    {
        using var document = JsonDocument.Parse(serviceAccountJson);
        return document.RootElement.TryGetProperty("project_id", out var projectId) && projectId.GetString() is { } value
            ? value
            : throw new InvalidOperationException("サービスアカウントJSONに project_id が見つかりません");
    }
}
