using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Application;

public interface IPracticeScheduleRepository
{
    /// <summary>start・end の両方を含む範囲(閉区間)で practices を取得する。</summary>
    Task<IReadOnlyList<PracticeSchedule>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
