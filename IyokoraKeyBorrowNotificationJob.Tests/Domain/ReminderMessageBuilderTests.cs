using IyokoraKeyBorrowNotificationJob.Domain;

namespace IyokoraKeyBorrowNotificationJob.Tests.Domain;

public class ReminderMessageBuilderTests
{
    [Fact]
    public void Build_FormatsDaysBeforeAndDate()
    {
        var practiceDate = new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc);

        var message = ReminderMessageBuilder.Build(3, practiceDate);

        Assert.Equal("【リマインド】3日後の練習(2026/04/05)の鍵の受け取りが未登録です。", message);
    }
}
