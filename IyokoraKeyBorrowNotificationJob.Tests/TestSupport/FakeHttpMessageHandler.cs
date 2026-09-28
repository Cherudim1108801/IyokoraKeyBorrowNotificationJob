using System.Net;

namespace IyokoraKeyBorrowNotificationJob.Tests.TestSupport;

/// <summary>実際に通信せず、直近のリクエストを記録して固定レスポンスを返す HttpMessageHandler。</summary>
public sealed class FakeHttpMessageHandler(HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastRequestBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        LastRequestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        return new HttpResponseMessage(statusCode);
    }
}
