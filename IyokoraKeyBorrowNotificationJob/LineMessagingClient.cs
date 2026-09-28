using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IyokoraKeyBorrowNotificationJob;

public sealed class LineMessagingClient(HttpClient httpClient, string channelAccessToken, string userId)
{
    private const string PushMessageUrl = "https://api.line.me/v2/bot/message/push";

    public async Task SendMessageAsync(string text, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, PushMessageUrl)
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken) },
            Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    to = userId,
                    messages = new[] { new { type = "text", text } }
                }),
                Encoding.UTF8,
                "application/json")
        };

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
