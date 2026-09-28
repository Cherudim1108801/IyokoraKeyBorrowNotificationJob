using System.Net;
using System.Text.Json;
using IyokoraKeyBorrowNotificationJob.Infrastructure;
using IyokoraKeyBorrowNotificationJob.Tests.TestSupport;

namespace IyokoraKeyBorrowNotificationJob.Tests.Infrastructure;

public class LineNotificationSenderTests
{
    [Fact]
    public async Task SendAsync_PostsExpectedRequest()
    {
        var handler = new FakeHttpMessageHandler();
        using var httpClient = new HttpClient(handler);
        var sender = new LineNotificationSender(httpClient, "channel-token", "user-123");

        await sender.SendAsync("こんにちは");

        var request = handler.LastRequest;
        Assert.NotNull(request);
        Assert.Equal(HttpMethod.Post, request!.Method);
        Assert.Equal("https://api.line.me/v2/bot/message/push", request.RequestUri!.ToString());
        Assert.Equal("Bearer", request.Headers.Authorization!.Scheme);
        Assert.Equal("channel-token", request.Headers.Authorization.Parameter);

        using var body = JsonDocument.Parse(handler.LastRequestBody!);
        Assert.Equal("user-123", body.RootElement.GetProperty("to").GetString());
        var message = body.RootElement.GetProperty("messages")[0];
        Assert.Equal("text", message.GetProperty("type").GetString());
        Assert.Equal("こんにちは", message.GetProperty("text").GetString());
    }

    [Fact]
    public async Task SendAsync_Throws_WhenResponseIsNotSuccess()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.BadRequest);
        using var httpClient = new HttpClient(handler);
        var sender = new LineNotificationSender(httpClient, "channel-token", "user-123");

        await Assert.ThrowsAsync<HttpRequestException>(() => sender.SendAsync("失敗するはず"));
    }
}
