using System.Text;
using Npgsql;

namespace log_to_db_performance;

public static class LogFileInserter
{
    public static void InsertLogEntries(string fileName, NpgsqlConnection conn, string[] batch, int itemsCount)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        var sqlCommand = new StringBuilder("INSERT INTO log (file_name, timestamp, thread_id, log_level, component, message) VALUES ");

        for (int i = 0; i < itemsCount; i++)
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
}