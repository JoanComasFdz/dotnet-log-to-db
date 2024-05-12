namespace log_to_db_parsing;

internal record LogEntry
{
    public DateTime Date { get; init; }
    public string Thread { get; init; }
    public string Level { get; init; }
    public string Resource { get; init; }
    public string Data { get; init; }

    public string? GetEventIpAddress { get; init; }
    public Guid? AcceptedQueryId { get; init; }
    public Guid? ExecutedQueryId { get; init; }
    public Guid? CompletedQueryId { get; init; }
    public long? CompletedQueryRows { get; init; }

    public static LogEntry Empty => new()
    {
        Date = DateTime.MinValue,
        Thread = string.Empty,
        Level = string.Empty,
        Resource = string.Empty,
        Data = string.Empty,
        GetEventIpAddress = string.Empty,
        AcceptedQueryId = Guid.Empty,
        ExecutedQueryId = Guid.Empty,
        CompletedQueryId = Guid.Empty,
        CompletedQueryRows = 0
    };

    public static LogEntry Create(
        DateTime timestamp,
        string threadId,
        string logLevel,
        string component,
        string message,
        string? getEventIpAddres,
        Guid? acceptedQueryId,
        Guid? executedQueryId,
        Guid? completedQueryId,
        long? completedRows) => new()
        {
            Date = timestamp,
            Thread = threadId,
            Level = logLevel,
            Resource = component,
            Data = message,
            GetEventIpAddress = getEventIpAddres,
            AcceptedQueryId = acceptedQueryId,
            ExecutedQueryId = executedQueryId,
            CompletedQueryId = completedQueryId,
            CompletedQueryRows = completedRows
        };

}