namespace IyokoraKeyBorrowNotificationJob.Tests;

public class ServiceAccountCredentialHelperTests
{
    [Fact]
    public void GetProjectId_ReturnsValue_WhenPresent()
    {
        const string json = """{"type":"service_account","project_id":"iyokoraattendanceapp"}""";

        var projectId = ServiceAccountCredentialHelper.GetProjectId(json);

        Assert.Equal("iyokoraattendanceapp", projectId);
    }

    [Fact]
    public void GetProjectId_Throws_WhenMissing()
    {
        const string json = """{"type":"service_account"}""";

        Assert.Throws<InvalidOperationException>(() => ServiceAccountCredentialHelper.GetProjectId(json));
    }
}
