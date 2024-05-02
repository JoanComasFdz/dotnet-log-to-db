using System.Globalization;

namespace log_to_db_performance_functional;

internal static class LogLineParser
{
    internal static IEnumerable<LogEntry> ToLogEntry(this IEnumerable<string> logLines)
    {
        return logLines.Select(ParseLogEntry);
    }

    internal static LogEntry ParseLogEntry(string logLine)
    {
        ReadOnlySpan<char> spanLine = logLine.AsSpan();

        // Find the end of the timestamp
        int firstSpaceIndex = spanLine.IndexOf(' ');
        if (firstSpaceIndex == -1)
        {
            return LogEntry.Empty; // Basic error handling
        }

        // Find the end of the thread_id (enclosed in brackets)
        int endThreadIdIndex = spanLine.IndexOf(']') + 1;
        if (endThreadIdIndex == 0)
        {
            return LogEntry.Empty; // Basic error handling
        }

        // Find the start of the log level by finding the space after thread_id
        int startLogLevelIndex = spanLine.Slice(endThreadIdIndex).IndexOf(' ') + endThreadIdIndex + 1;
        if (startLogLevelIndex <= endThreadIdIndex)
        {
            return LogEntry.Empty; // Basic error handling
        }

        // Find the end of the log level by finding the space after the log level start
        int endLogLevelIndex = spanLine.Slice(startLogLevelIndex).IndexOf(" ") + startLogLevelIndex;
        if (endLogLevelIndex <= startLogLevelIndex)
        {
            return LogEntry.Empty; // Basic error handling
        }

        int endLogLevelIndex2 = spanLine.Slice(endLogLevelIndex).IndexOf(" ");
        if (endLogLevelIndex2 - endLogLevelIndex == 1)
        {
            endLogLevelIndex = endLogLevelIndex2; // If there is a double space, use the second space as the end of the log level
        }

        // Find the end of the component by finding the space after the log level end
        int endComponentIndex = spanLine.Slice(endLogLevelIndex).IndexOf(" - ") + endLogLevelIndex;
        if (endComponentIndex <= endLogLevelIndex)
        {
            return LogEntry.Empty; // Basic error handling
        }

        // Assigning segments to variables
        ReadOnlySpan<char> timestampSpan = spanLine.Slice(0, firstSpaceIndex);
        ReadOnlySpan<char> threadIdSpan = spanLine.Slice(firstSpaceIndex + 2, endThreadIdIndex - firstSpaceIndex - 3);
        ReadOnlySpan<char> logLevelSpan = spanLine.Slice(startLogLevelIndex, endLogLevelIndex - startLogLevelIndex);
        ReadOnlySpan<char> componentSpan = spanLine.Slice(endLogLevelIndex+2, endComponentIndex - endLogLevelIndex - 2);
        ReadOnlySpan<char> messageSpan = spanLine.Slice(endComponentIndex + 3);

        var timestamp = DateTime.ParseExact(timestampSpan.ToString(), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None);

        return LogEntry.Create(
            timestamp,
            threadIdSpan.ToString(),
            logLevelSpan.ToString(),
            componentSpan.ToString(),
            messageSpan.ToString());
    }
}
