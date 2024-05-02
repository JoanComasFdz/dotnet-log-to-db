namespace log_to_db_performance_functional;

internal record LogEntry
{
    public DateTime Timestamp { get; init; }
    public string ThreadId { get; init; }
    public string LogLevel { get; init; }
    public string Component { get; init; }
    public string Message { get; init; }

    public static LogEntry Empty => new()
    {
        Timestamp = DateTime.MinValue,
        ThreadId = string.Empty,
        LogLevel = string.Empty,
        Component = string.Empty,
        Message = string.Empty
    };

    public static LogEntry Create(DateTime timestamp, string threadId, string logLevel, string component, string message) =>
        new()
        {
            Timestamp = timestamp,
            ThreadId = threadId,
            LogLevel = logLevel,
            Component = component,
            Message = message
    };

}