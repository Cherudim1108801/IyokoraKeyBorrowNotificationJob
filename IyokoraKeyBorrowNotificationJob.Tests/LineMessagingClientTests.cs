using System.Net;
using System.Text.Json;
using IyokoraKeyBorrowNotificationJob.Tests.TestSupport;

namespace IyokoraKeyBorrowNotificationJob.Tests;

public class LineMessagingClientTests
{
    [Fact]
    public async Task SendMessageAsync_PostsExpectedRequest()
    {
        var handler = new FakeHttpMessageHandler();
        using var httpClient = new HttpClient(handler);
        var client = new LineMessagingClient(httpClient, "channel-token", "user-123");

        await client.SendMessageAsync("こんにちは");

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
    public async Task SendMessageAsync_Throws_WhenResponseIsNotSuccess()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.BadRequest);
        using var httpClient = new HttpClient(handler);
        var client = new LineMessagingClient(httpClient, "channel-token", "user-123");

        await Assert.ThrowsAsync<HttpRequestException>(() => client.SendMessageAsync("失敗するはず"));
    }
}
