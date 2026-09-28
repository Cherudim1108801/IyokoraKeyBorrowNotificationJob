using IyokoraKeyBorrowNotificationJob.Application;
using IyokoraKeyBorrowNotificationJob.Domain;
using IyokoraKeyBorrowNotificationJob.Tests.TestSupport;

namespace IyokoraKeyBorrowNotificationJob.Tests.Application;

public class SendKeyPickupRemindersUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_SendsReminder_OnlyForSchedulesNeedingPickup()
    {
        var utcNow = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);
        var targetDate = new DateTime(2026, 4, 4, 0, 0, 0, DateTimeKind.Utc);

        var schedules = new List<PracticeSchedule>
        {
            new("p1", targetDate, RequiresKeyPickup: true, KeyPickedUp: false),
            new("p2", targetDate, RequiresKeyPickup: true, KeyPickedUp: true),
            new("p3", targetDate, RequiresKeyPickup: false, KeyPickedUp: false),
        };
        var repository = new FakePracticeScheduleRepository(schedules);
        var sender = new FakeNotificationSender();
        var useCase = new SendKeyPickupRemindersUseCase(repository, sender);

        var result = await useCase.ExecuteAsync(utcNow, daysBefore: 3);

        Assert.Equal(targetDate, repository.RequestedDate);
        var message = Assert.Single(sender.SentMessages);
        Assert.Equal("【リマインド】3日後の練習(2026/04/04)の鍵の受け取りが未登録です。", message);

        Assert.Equal(targetDate, result.TargetDate);
        Assert.Equal(3, result.SchedulesFound);
        Assert.Equal(1, result.RemindersSent);
    }

    [Fact]
    public async Task ExecuteAsync_SendsNothing_WhenNoScheduleNeedsReminder()
    {
        var utcNow = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);
        var targetDate = utcNow.AddDays(3);

        var schedules = new List<PracticeSchedule>
        {
            new("p1", targetDate, RequiresKeyPickup: false, KeyPickedUp: false),
            new("p2", targetDate, RequiresKeyPickup: true, KeyPickedUp: true),
        };
        var sender = new FakeNotificationSender();
        var useCase = new SendKeyPickupRemindersUseCase(new FakePracticeScheduleRepository(schedules), sender);

        var result = await useCase.ExecuteAsync(utcNow, daysBefore: 3);

        Assert.Empty(sender.SentMessages);
        Assert.Equal(2, result.SchedulesFound);
        Assert.Equal(0, result.RemindersSent);
    }

    [Fact]
    public async Task ExecuteAsync_SendsNothing_WhenNoSchedulesOnTargetDate()
    {
        var utcNow = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);
        var sender = new FakeNotificationSender();
        var useCase = new SendKeyPickupRemindersUseCase(
            new FakePracticeScheduleRepository([]),
            sender);

        var result = await useCase.ExecuteAsync(utcNow, daysBefore: 3);

        Assert.Empty(sender.SentMessages);
        Assert.Equal(0, result.SchedulesFound);
        Assert.Equal(0, result.RemindersSent);
    }
}
