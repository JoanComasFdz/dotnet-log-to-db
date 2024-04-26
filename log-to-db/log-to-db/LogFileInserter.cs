using System.Globalization;
using System.Text;
using Npgsql;

namespace log_to_db;

public static class LogFileInserter
{
    private const int BatchSize = 500;
    private static readonly string[] batch = new string[BatchSize];
    private static int currentIndex = 0;

    public static void ProcessLogFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        using var conn = new NpgsqlConnection(Database.ConnectionString);
        conn.Open();

        using var reader = new StreamReader(filePath);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var spanLine = line.AsSpan();
            if (LineStartsWithWhiteSpace(spanLine) || !FirstCharacterIsDigit(spanLine) || !LineStartsWithDateTime(spanLine))
                continue;

            batch[currentIndex++] = line;
            if (currentIndex == BatchSize)
            {
                InsertLogEntries(fileName, conn);
                currentIndex = 0;
            }
        }

        if (currentIndex > 0)
        {
            InsertLogEntries(fileName, conn);
        }
    }

    private static void InsertLogEntries(string fileName, NpgsqlConnection conn)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        var sqlCommand = new StringBuilder("INSERT INTO log (file_name, timestamp, thread_id, log_level, component, message) VALUES ");

        for (int i = 0; i < currentIndex; i++)
        {
            var entry = LogLineParser.ParseLogEntry(batch[i]);
            if (i > 0)
                sqlCommand.Append(", ");
            sqlCommand.Append($"(@file_name{i}, @timestamp{i}, @thread_id{i}, @log_level{i}, @component{i}, @message{i})");

            cmd.Parameters.AddWithValue($"file_name{i}", fileName);
            cmd.Parameters.AddWithValue($"timestamp{i}", entry.Timestamp);
            cmd.Parameters.AddWithValue($"thread_id{i}", entry.ThreadId.ToString());
            cmd.Parameters.AddWithValue($"log_level{i}", entry.LogLevel.ToString());
            cmd.Parameters.AddWithValue($"component{i}", entry.Component.ToString());
            cmd.Parameters.AddWithValue($"message{i}", entry.Message.ToString());
        }

        cmd.CommandText = sqlCommand.ToString();
        cmd.ExecuteNonQuery();
    }

    private static bool LineStartsWithDateTime(ReadOnlySpan<char> line)
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

    private static bool LineStartsWithWhiteSpace(ReadOnlySpan<char> line)
    {
        // Check if the line starts with either a whitespace or a tab character
        return line.Length > 0 && (line[0] == ' ' || line[0] == '\t');
    }

    private static bool FirstCharacterIsDigit(ReadOnlySpan<char> line)
    {
        return line.Length > 0 && char.IsDigit(line[0]);
    }
}