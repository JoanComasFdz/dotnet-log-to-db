namespace log_to_db_parsing;

internal static class LogLineValidator
{
    public static IEnumerable<string> WhereLineIsRelevant(this IEnumerable<string> lines)
    {
        return lines.Where(line => LineStartsWithDateTime(line));
    }

    public static bool LineStartsWithDateTime(ReadOnlySpan<char> line)
    {
        // Example specific check for format "2024-12-13T12:34:56.798"
        return line.Length >= 23 &&
            char.IsDigit(line[0]) && char.IsDigit(line[1]) && char.IsDigit(line[2]) && char.IsDigit(line[3]) &&  // Year part
            line[4] == '-' &&
            char.IsDigit(line[5]) && char.IsDigit(line[6]) &&  // Month part
            line[7] == '-' &&
            char.IsDigit(line[8]) && char.IsDigit(line[9]) &&  // Day part
            line[10] == 'T' &&
            char.IsDigit(line[11]) && char.IsDigit(line[12]) &&  // Hour part
            line[13] == ':' &&
            char.IsDigit(line[14]) && char.IsDigit(line[15]) &&  // Minute part
            line[16] == ':' &&
            char.IsDigit(line[17]) && char.IsDigit(line[18]) &&  // Second part
            line[19] == '.' &&
            char.IsDigit(line[20]) && char.IsDigit(line[21]) && char.IsDigit(line[22]);  // Millisecond part
    }
}