namespace IyokoraKeyBorrowNotificationJob.Application;

public interface INotificationSender
{
    Task SendAsync(string message, CancellationToken cancellationToken = default);
}
