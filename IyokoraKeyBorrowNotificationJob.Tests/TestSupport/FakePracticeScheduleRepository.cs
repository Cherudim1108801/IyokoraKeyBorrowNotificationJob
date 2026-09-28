using IyokoraKeyBorrowNotificationJob.Application;
using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Tests.TestSupport;

public sealed class FakePracticeScheduleRepository(IReadOnlyList<PracticeSchedule> schedules) : IPracticeScheduleRepository
{
    public (DateTime Start, DateTime End)? RequestedRange { get; private set; }

    public Task<IReadOnlyList<PracticeSchedule>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        RequestedRange = (start, end);
        return Task.FromResult(schedules);
    }
}
