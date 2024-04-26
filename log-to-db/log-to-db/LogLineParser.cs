using System.Globalization;

internal static class LogLineParser
{
    internal static LogEntry ParseLogEntry(string logLine)
    {
        ReadOnlySpan<char> spanLine = logLine.AsSpan();

        // Find the end of the timestamp
        int firstSpaceIndex = spanLine.IndexOf(' ');
        if (firstSpaceIndex == -1) return LogEntry.Empty; // Basic error handling

        // Find the end of the thread_id (enclosed in brackets)
        int endThreadIdIndex = spanLine.IndexOf(']') + 1;
        if (endThreadIdIndex == 0) return LogEntry.Empty; // Basic error handling

        // Find the start of the log level by finding the space after thread_id
        int startLogLevelIndex = spanLine.Slice(endThreadIdIndex).IndexOf(' ') + endThreadIdIndex + 1;
        if (startLogLevelIndex <= endThreadIdIndex) return LogEntry.Empty; // Basic error handling

        // Find the end of the log level by finding the space after the log level start
        int endLogLevelIndex = spanLine.Slice(startLogLevelIndex).IndexOf(' ') + startLogLevelIndex;
        if (endLogLevelIndex <= startLogLevelIndex) return LogEntry.Empty; // Basic error handling

        // Find the end of the component by finding the space after the log level end
        int endComponentIndex = spanLine.Slice(endLogLevelIndex).IndexOf(" - ") + endLogLevelIndex;
        if (endComponentIndex <= endLogLevelIndex) return LogEntry.Empty; // Basic error handling

        // Assigning segments to variables
        ReadOnlySpan<char> timestampSpan = spanLine.Slice(0, firstSpaceIndex);
        ReadOnlySpan<char> threadIdSpan = spanLine.Slice(firstSpaceIndex + 1, endThreadIdIndex - firstSpaceIndex - 1);
        ReadOnlySpan<char> logLevelSpan = spanLine.Slice(startLogLevelIndex, endLogLevelIndex - startLogLevelIndex);
        ReadOnlySpan<char> componentSpan = spanLine.Slice(endLogLevelIndex +1, endComponentIndex - endLogLevelIndex);
        ReadOnlySpan<char> messageSpan = spanLine.Slice(endComponentIndex + 3);

        var timestamp = DateTime.ParseExact(timestampSpan.ToString(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None);

        return new(timestamp, threadIdSpan, logLevelSpan, componentSpan, messageSpan);
    }
}

internal ref struct LogEntry(DateTime timestamp, ReadOnlySpan<char> threadId, ReadOnlySpan<char> logLevel, ReadOnlySpan<char> component, ReadOnlySpan<char> message)
{
    public DateTime Timestamp = timestamp;
    public ReadOnlySpan<char> ThreadId = threadId;
    public ReadOnlySpan<char> LogLevel = logLevel;
    public ReadOnlySpan<char> Component = component;
    public ReadOnlySpan<char> Message = message;

    public static LogEntry Empty => new(DateTime.MinValue, string.Empty, string.Empty, string.Empty, string.Empty);
}