using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Application;

public sealed class SendKeyPickupRemindersUseCase(
    IPracticeScheduleRepository practiceScheduleRepository,
    INotificationSender notificationSender)
{
    /// <summary>
    /// lookAheadDays 日以内に登録されている practice のうち、最も直近の日付のものを対象に
    /// 鍵の受け取りが未登録であればリマインドを送信する。
    /// </summary>
    public async Task<SendKeyPickupRemindersResult> ExecuteAsync(DateTime utcNow, int lookAheadDays, CancellationToken cancellationToken = default)
    {
        var (start, end) = PracticeSearchRangeCalculator.GetRange(utcNow, lookAheadDays);
        var schedules = await practiceScheduleRepository.GetByDateRangeAsync(start, end, cancellationToken);

        if (schedules.Count == 0)
        {
            return new SendKeyPickupRemindersResult(null, 0, 0);
        }

        var nearestDate = schedules.Min(s => s.Date);
        var nearestSchedules = schedules.Where(s => s.Date == nearestDate);

        var remindersSent = 0;
        foreach (var schedule in nearestSchedules)
        {
            if (!schedule.NeedsReminder)
            {
                continue;
            }

            var daysUntil = (schedule.Date.Date - start.Date).Days;
            await notificationSender.SendAsync(
                ReminderMessageBuilder.Build(daysUntil, schedule.Date),
                cancellationToken);
            remindersSent++;
        }

        return new SendKeyPickupRemindersResult(nearestDate, schedules.Count, remindersSent);
    }
}
