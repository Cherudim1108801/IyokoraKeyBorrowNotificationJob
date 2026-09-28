using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Application;

public sealed class SendKeyPickupRemindersUseCase(
    IPracticeScheduleRepository practiceScheduleRepository,
    INotificationSender notificationSender)
{
    public async Task ExecuteAsync(DateTime utcNow, int daysBefore, CancellationToken cancellationToken = default)
    {
        var targetDate = PracticeTargetDateCalculator.GetTargetDate(utcNow, daysBefore);
        var schedules = await practiceScheduleRepository.GetByDateAsync(targetDate, cancellationToken);

        foreach (var schedule in schedules)
        {
            if (!schedule.NeedsReminder)
            {
                continue;
            }

            await notificationSender.SendAsync(
                ReminderMessageBuilder.Build(daysBefore, schedule.Date),
                cancellationToken);
        }
    }
}
