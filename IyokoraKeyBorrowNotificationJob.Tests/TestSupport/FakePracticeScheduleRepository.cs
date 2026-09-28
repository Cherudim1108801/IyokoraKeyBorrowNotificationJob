using IyokoraKeyBorrowNotificationJob.Application;
using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Tests.TestSupport;

public sealed class FakePracticeScheduleRepository(IReadOnlyList<PracticeSchedule> schedules) : IPracticeScheduleRepository
{
    public DateTime? RequestedDate { get; private set; }

    public Task<IReadOnlyList<PracticeSchedule>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        RequestedDate = date;
        return Task.FromResult(schedules);
    }
}
