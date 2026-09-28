using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Application;

public interface IPracticeScheduleRepository
{
    Task<IReadOnlyList<PracticeSchedule>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
}
