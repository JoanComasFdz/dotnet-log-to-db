namespace log_to_db_performance;

internal static class LogLineValidator
{
    public static bool LineStartsWithDateTime(ReadOnlySpan<char> line)
    {
        // Example specific check for format "2024-04-17T00:00:01.858"
        // Check sufficient length and structural characters
        return line.Length >= 23 &&
               line[4] == '-' &&
               line[7] == '-' &&
               line[10] == 'T' &&
               line[13] == ':' &&
               line[16] == ':' &&
               line[19] == '.' &&
               char.IsDigit(line[0]) && char.IsDigit(line[1]) && char.IsDigit(line[2]) && char.IsDigit(line[3]) &&  // Year part
               char.IsDigit(line[5]) && char.IsDigit(line[6]) &&  // Month part
               char.IsDigit(line[8]) && char.IsDigit(line[9]) &&  // Day part
               char.IsDigit(line[11]) && char.IsDigit(line[12]) &&  // Hour part
               char.IsDigit(line[14]) && char.IsDigit(line[15]) &&  // Minute part
               char.IsDigit(line[17]) && char.IsDigit(line[18]) &&  // Second part
               char.IsDigit(line[20]) && char.IsDigit(line[21]) && char.IsDigit(line[22]);  // Millisecond part
    }
}