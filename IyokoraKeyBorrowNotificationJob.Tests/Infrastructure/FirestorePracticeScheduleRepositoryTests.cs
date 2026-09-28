using Google.Cloud.Firestore;
using IyokoraKeyBorrowNotificationJob.Infrastructure;

namespace IyokoraKeyBorrowNotificationJob.Tests.Infrastructure;

public class FirestorePracticeScheduleRepositoryTests
{
    [Fact]
    public void ToPracticeSchedule_MapsAllFields_WhenPresent()
    {
        var date = new DateTime(2026, 4, 4, 0, 0, 0, DateTimeKind.Utc);
        var data = new Dictionary<string, object>
        {
            ["date"] = Timestamp.FromDateTime(date),
            ["requiresKeyPickup"] = true,
            ["keyPickedUp"] = false,
        };

        var schedule = FirestorePracticeScheduleRepository.ToPracticeSchedule("p1", data);

        Assert.Equal("p1", schedule.Id);
        Assert.Equal(date, schedule.Date);
        Assert.True(schedule.RequiresKeyPickup);
        Assert.False(schedule.KeyPickedUp);
    }

    [Fact]
    public void ToPracticeSchedule_TreatsMissingBooleanFields_AsFalse()
    {
        var date = new DateTime(2026, 4, 4, 0, 0, 0, DateTimeKind.Utc);
        var data = new Dictionary<string, object>
        {
            ["date"] = Timestamp.FromDateTime(date),
        };

        var schedule = FirestorePracticeScheduleRepository.ToPracticeSchedule("p1", data);

        Assert.False(schedule.RequiresKeyPickup);
        Assert.False(schedule.KeyPickedUp);
    }

    [Fact]
    public void ToPracticeSchedule_Throws_WhenDateFieldIsMissing()
    {
        var data = new Dictionary<string, object>
        {
            ["requiresKeyPickup"] = true,
        };

        Assert.Throws<KeyNotFoundException>(() => FirestorePracticeScheduleRepository.ToPracticeSchedule("p1", data));
    }
}
