using IyokoraKeyBorrowNotificationJob.Application;

namespace IyokoraKeyBorrowNotificationJob.Tests.TestSupport;

public sealed class FakeNotificationSender : INotificationSender
{
    public List<string> SentMessages { get; } = [];

    public Task SendAsync(string message, CancellationToken cancellationToken = default)
    {
        SentMessages.Add(message);
        return Task.CompletedTask;
    }
}
