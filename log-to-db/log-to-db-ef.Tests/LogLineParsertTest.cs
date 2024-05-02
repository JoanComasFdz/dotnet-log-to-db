namespace log_to_db_ef.Tests;

public class LogLineParsertTest
{
    [Fact]
    public void StandardLine_IsParsedCorrectly()
    {
        const string fileName ="log-file-name.log";
        const string logLine = "2021-08-01T00:00:00.000 [some-thread-1] INFO  namespace.nested.componentName - message";

        var logEntry = LogLineParser.ParseLogEntry(logLine, fileName);
            
        var correctLogEntry = new LogEntry(
            fileName,
            new DateTime(2021, 8, 1, 0, 0, 0, 0),
            "some-thread-1",
            "INFO",
            "namespace.nested.componentName",
            "message"
            );

        Assert.Equal(correctLogEntry, logEntry);
    }

    [Fact]
    public void GET_IsParsedCorrectly()
    {
        const string fileName = "log-file-name.log";
        const string logLine = "2021-08-01T00:00:00.000 [some-thread-1] INFO  namespace.nested.componentName - GET https://10.90.90.19:5555/api/v3/resource/AResult/slice?from=20240417000209&to=20240417000239";

        var logEntry = LogLineParser.ParseLogEntry(logLine, fileName);

        var correctLogEntry = new LogEntry(
            fileName,
            new DateTime(2021, 8, 1, 0, 0, 0, 0),
            "some-thread-1",
            "INFO",
            "namespace.nested.componentName",
            "GET https://10.90.90.19:5555/api/v3/resource/AResult/slice?from=20240417000209&to=20240417000239"
            );

        Assert.Equal(correctLogEntry, logEntry);
    }

    [Fact]
    public void ERROR_IsParsedCorrectly()
    {
        const string fileName = "log-file-name.log";
        const string logLine = "2021-08-01T00:00:00.000 [reactor-http-nio-4] ERROR  namespace.nested.componentName - [95f9bbe7] 500 Server Error for HTTP GET \"/api/v3/resource/SomeResult/slice?from=20240308034957&to=20240308035057\"";

        var logEntry = LogLineParser.ParseLogEntry(logLine, fileName);

        var correctLogEntry = new LogEntry(
            fileName,
            new DateTime(2021, 8, 1, 0, 0, 0, 0),
            "reactor-http-nio-4",
            "ERROR",
            "namespace.nested.componentName",
            "[95f9bbe7] 500 Server Error for HTTP GET \"/api/v3/resource/SomeResult/slice?from=20240308034957&to=20240308035057\""
            );

        Assert.Equal(correctLogEntry, logEntry);
    }

    [Fact]
    public void Short_IsParsedCorrectly()
    {
        const string fileName = "log-file-name.log";
        const string logLine = "2021-08-01T00:00:00.000 [parallel - 8] INFO  namespace.nested.componentName - query#25d67e17-2e01-4c75-9089-9f22a4707b70 completed with 0 rows";

        var logEntry = LogLineParser.ParseLogEntry(logLine, fileName);

        var correctLogEntry = new LogEntry(
            fileName,
            new DateTime(2021, 8, 1, 0, 0, 0, 0),
            "parallel - 8",
            "INFO",
            "namespace.nested.componentName",
            "query#25d67e17-2e01-4c75-9089-9f22a4707b70 completed with 0 rows"
            );
        
        Assert.Equal(correctLogEntry, logEntry);
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
        const string fileName = "log-file-name.log";

        var logEntry = LogLineParser.ParseLogEntry(logLine, fileName);

        Assert.Equal(LogEntry.Empty, logEntry);
    }
}
