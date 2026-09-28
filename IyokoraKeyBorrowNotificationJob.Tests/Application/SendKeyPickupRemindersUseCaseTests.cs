using IyokoraKeyBorrowNotificationJob.Application;
using IyokoraKeyBorrowNotificationJob.Domain;
using IyokoraKeyBorrowNotificationJob.Tests.TestSupport;

namespace IyokoraKeyBorrowNotificationJob.Tests.Application;

public class SendKeyPickupRemindersUseCaseTests
{
    private static readonly DateTime UtcNow = new(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ExecuteAsync_SendsReminder_ForNearestScheduleNeedingPickup()
    {
        var nearestDate = new DateTime(2026, 4, 3, 0, 0, 0, DateTimeKind.Utc);
        var laterDate = new DateTime(2026, 4, 6, 0, 0, 0, DateTimeKind.Utc);

        var schedules = new List<PracticeSchedule>
        {
            new("p1", nearestDate, RequiresKeyPickup: true, KeyPickedUp: false),
            new("p2", laterDate, RequiresKeyPickup: true, KeyPickedUp: false),
        };
        var repository = new FakePracticeScheduleRepository(schedules);
        var sender = new FakeNotificationSender();
        var useCase = new SendKeyPickupRemindersUseCase(repository, sender);

        var result = await useCase.ExecuteAsync(UtcNow, lookAheadDays: 7);

        Assert.Equal((UtcNow, UtcNow.AddDays(7)), repository.RequestedRange);
        var message = Assert.Single(sender.SentMessages);
        Assert.Equal("【リマインド】2日後の練習(2026/04/03)の鍵の受け取りが未登録です。", message);

        Assert.Equal(nearestDate, result.NearestPracticeDate);
        Assert.Equal(2, result.SchedulesInRange);
        Assert.Equal(1, result.RemindersSent);
    }

    [Fact]
    public async Task ExecuteAsync_SendsNothing_WhenNearestScheduleDoesNotNeedPickup_EvenIfLaterOneDoes()
    {
        var nearestDate = new DateTime(2026, 4, 3, 0, 0, 0, DateTimeKind.Utc);
        var laterDate = new DateTime(2026, 4, 6, 0, 0, 0, DateTimeKind.Utc);

        var schedules = new List<PracticeSchedule>
        {
            new("p1", nearestDate, RequiresKeyPickup: false, KeyPickedUp: false),
            new("p2", laterDate, RequiresKeyPickup: true, KeyPickedUp: false),
        };
        var sender = new FakeNotificationSender();
        var useCase = new SendKeyPickupRemindersUseCase(new FakePracticeScheduleRepository(schedules), sender);

        var result = await useCase.ExecuteAsync(UtcNow, lookAheadDays: 7);

        Assert.Empty(sender.SentMessages);
        Assert.Equal(nearestDate, result.NearestPracticeDate);
        Assert.Equal(0, result.RemindersSent);
    }

    [Fact]
    public async Task ExecuteAsync_SendsReminderForEach_WhenMultipleSchedulesShareNearestDate()
    {
        var nearestDate = new DateTime(2026, 4, 3, 0, 0, 0, DateTimeKind.Utc);

        var schedules = new List<PracticeSchedule>
        {
            new("p1", nearestDate, RequiresKeyPickup: true, KeyPickedUp: false),
            new("p2", nearestDate, RequiresKeyPickup: true, KeyPickedUp: false),
            new("p3", nearestDate, RequiresKeyPickup: true, KeyPickedUp: true),
        };
        var sender = new FakeNotificationSender();
        var useCase = new SendKeyPickupRemindersUseCase(new FakePracticeScheduleRepository(schedules), sender);

        var result = await useCase.ExecuteAsync(UtcNow, lookAheadDays: 7);

        Assert.Equal(2, sender.SentMessages.Count);
        Assert.Equal(2, result.RemindersSent);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNullNearestDate_WhenNoSchedulesInRange()
    {
        var sender = new FakeNotificationSender();
        var useCase = new SendKeyPickupRemindersUseCase(new FakePracticeScheduleRepository([]), sender);

        var result = await useCase.ExecuteAsync(UtcNow, lookAheadDays: 7);

        Assert.Empty(sender.SentMessages);
        Assert.Null(result.NearestPracticeDate);
        Assert.Equal(0, result.SchedulesInRange);
        Assert.Equal(0, result.RemindersSent);
    }
}
