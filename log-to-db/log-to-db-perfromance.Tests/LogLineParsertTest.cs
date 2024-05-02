namespace log_to_db_performance.Tests;

public class LogLineParsertTest
{
    [Fact]
    public void StandardLine_IsParsedCorrectly()
    {
        const string logLine = "2021-08-01T00:00:00.000 [some-thread-1] INFO  namespace.nested.ComponentName - Message";

        var logEntry = LogLineParser.ParseLogEntry(logLine);
            
        var correctLogEntry = new LogEntry(
            new DateTime(2021, 8, 1, 0, 0, 0, 0),
            "some-thread-1",
            "INFO",
            "namespace.nested.ComponentName",
            "Message"
            );

        Assert.Equal(correctLogEntry.Timestamp, logEntry.Timestamp);
        Assert.Equal(correctLogEntry.ThreadId.ToString(), logEntry.ThreadId.ToString());
        Assert.Equal(correctLogEntry.LogLevel.ToString(), logEntry.LogLevel.ToString());
        Assert.Equal(correctLogEntry.Component.ToString(), logEntry.Component.ToString());
        Assert.Equal(correctLogEntry.Message.ToString(), logEntry.Message.ToString());
    }

    [Fact]
    public void GET_IsParsedCorrectly()
    {
        const string logLine = "2021-08-01T00:00:00.000 [some-thread-1] INFO  namespace.nested.ComponentName - GET https://10.90.90.19:5555/api/v3/resource/AResult/slice?from=20240417000209&to=20240417000239";

        var logEntry = LogLineParser.ParseLogEntry(logLine);

        var correctLogEntry = new LogEntry(
            new DateTime(2021, 8, 1, 0, 0, 0, 0),
            "some-thread-1",
            "INFO",
            "namespace.nested.ComponentName",
            "GET https://10.90.90.19:5555/api/v3/resource/AResult/slice?from=20240417000209&to=20240417000239"
            );

        Assert.Equal(correctLogEntry.Timestamp, logEntry.Timestamp);
        Assert.Equal(correctLogEntry.ThreadId.ToString(), logEntry.ThreadId.ToString());
        Assert.Equal(correctLogEntry.LogLevel.ToString(), logEntry.LogLevel.ToString());
        Assert.Equal(correctLogEntry.Component.ToString(), logEntry.Component.ToString());
        Assert.Equal(correctLogEntry.Message.ToString(), logEntry.Message.ToString());
    }

    [Fact]
    public void ERROR_IsParsedCorrectly()
    {
        const string logLine = "2021-08-01T00:00:00.000 [reactor-http-nio-4] ERROR  namespace.nested.ComponentName - [95f9bbe7] 500 Server Error for HTTP GET \"/api/v3/resource/SomeResult/slice?from=20240308034957&to=20240308035057\"";

        var logEntry = LogLineParser.ParseLogEntry(logLine);

        var correctLogEntry = new LogEntry(
            new DateTime(2021, 8, 1, 0, 0, 0, 0),
            "reactor-http-nio-4",
            "ERROR",
            "namespace.nested.ComponentName",
            "[95f9bbe7] 500 Server Error for HTTP GET \"/api/v3/resource/SomeResult/slice?from=20240308034957&to=20240308035057\""
            );

        Assert.Equal(correctLogEntry.Timestamp, logEntry.Timestamp);
        Assert.Equal(correctLogEntry.ThreadId.ToString(), logEntry.ThreadId.ToString());
        Assert.Equal(correctLogEntry.LogLevel.ToString(), logEntry.LogLevel.ToString());
        Assert.Equal(correctLogEntry.Component.ToString(), logEntry.Component.ToString());
        Assert.Equal(correctLogEntry.Message.ToString(), logEntry.Message.ToString());
    }

    [Fact]
    public void Short_IsParsedCorrectly()
    {
        const string logLine = "2021-08-01T00:00:00.000 [parallel - 8] INFO  namespace.nested.ComponentName - query#25d67e17-2e01-4c75-9089-9f22a4707b70 completed with 0 rows";

        var logEntry = LogLineParser.ParseLogEntry(logLine);

        var correctLogEntry = new LogEntry(
            new DateTime(2021, 8, 1, 0, 0, 0, 0),
            "parallel - 8",
            "INFO",
            "namespace.nested.ComponentName",
            "query#25d67e17-2e01-4c75-9089-9f22a4707b70 completed with 0 rows"
            );

        Assert.Equal(correctLogEntry.Timestamp, logEntry.Timestamp);
        Assert.Equal(correctLogEntry.ThreadId.ToString(), logEntry.ThreadId.ToString());
        Assert.Equal(correctLogEntry.LogLevel.ToString(), logEntry.LogLevel.ToString());
        Assert.Equal(correctLogEntry.Component.ToString(), logEntry.Component.ToString());
        Assert.Equal(correctLogEntry.Message.ToString(), logEntry.Message.ToString());
    }

    [Theory]
    [InlineData("")] // Empty string
    [InlineData(" ")] // Space
    [InlineData("}")] // Closing JSON bracket
    [InlineData("/")] // ASCII drawing
    [InlineData("   ")] // Tab
    [InlineData("Version: 1.2.3.4")] // Restart
    [InlineData("Using")] // Isolated message
    [InlineData("javax")] // Exception stack trace
    [InlineData("org")] // Exception stack trace
    public void IncorrectStart_IsParsedCorrectly(string logLine)
    {
        var logEntry = LogLineParser.ParseLogEntry(logLine);

        Assert.Equal(LogEntry.Empty.Timestamp, logEntry.Timestamp);
        Assert.Equal(LogEntry.Empty.ThreadId.ToString(), logEntry.ThreadId.ToString());
        Assert.Equal(LogEntry.Empty.LogLevel.ToString(), logEntry.LogLevel.ToString());
        Assert.Equal(LogEntry.Empty.Component.ToString(), logEntry.Component.ToString());
        Assert.Equal(LogEntry.Empty.Message.ToString(), logEntry.Message.ToString());
    }
}
