using log_to_db_performance_functional;
using Npgsql;
using System.Text;

internal static class SQLCommandCreator
{
    public static IEnumerable<NpgsqlCommand> ToSQLCommands(this IEnumerable<LogEntry[]> logEntryBatches, string fileName)
    {
        foreach (var batch in logEntryBatches)
        {
            if (batch.Length == 0)
            { 
                continue;
            }

            yield return ToSQLCommand(fileName, batch);
        }
    }

    public static NpgsqlCommand ToSQLCommand(string fileName, LogEntry[] batch)
    {
        var cmd = new NpgsqlCommand();
        var sqlCommand = new StringBuilder("INSERT INTO log (file_name, timestamp, thread_id, log_level, component, message) VALUES ");
        var i = 0;
        foreach (var entry in batch)
        {
            if (i++ > 0)
            {
                sqlCommand.Append(", ");
            }

            sqlCommand.Append($"(@file_name{i}, @timestamp{i}, @thread_id{i}, @log_level{i}, @component{i}, @message{i})");

            cmd.Parameters.AddWithValue($"file_name{i}", fileName);
            cmd.Parameters.AddWithValue($"timestamp{i}", entry.Timestamp);
            cmd.Parameters.AddWithValue($"thread_id{i}", entry.ThreadId.ToString());
            cmd.Parameters.AddWithValue($"log_level{i}", entry.LogLevel.ToString());
            cmd.Parameters.AddWithValue($"component{i}", entry.Component.ToString());
            cmd.Parameters.AddWithValue($"message{i}", entry.Message.ToString());
        }

        cmd.CommandText = sqlCommand.ToString();
        return cmd;
    }
}
