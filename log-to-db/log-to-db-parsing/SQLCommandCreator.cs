using Npgsql;
using System.Text;

namespace log_to_db_parsing;

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
        var sqlCommand = new StringBuilder("INSERT INTO log (file_name, date, thread, level, resource, data, ip_address, query_id_accepted, query_id_executed, query_id_completed, completed_rows) VALUES ");
        var i = 0;
        foreach (var entry in batch)
        {
            if (i++ > 0)
            {
                sqlCommand.Append(", ");
            }

            sqlCommand.Append($"(@file_name{i}, @date{i}, @thread{i}, @level{i}, @resource{i}, @data{i}, @ip_address{i}, @query_id_accepted{i}, @query_id_executed{i}, @query_id_completed{i}, @completed_rows{i})");

            cmd.Parameters.AddWithValue($"file_name{i}", fileName);
            cmd.Parameters.AddWithValue($"date{i}", entry.Date);
            cmd.Parameters.AddWithValue($"thread{i}", entry.Thread.ToString());
            cmd.Parameters.AddWithValue($"level{i}", entry.Level.ToString());
            cmd.Parameters.AddWithValue($"resource{i}", entry.Resource.ToString());
            cmd.Parameters.AddWithValue($"data{i}", entry.Data.ToString());
            cmd.Parameters.AddWithValueOrTypeIfNull($"ip_address{i}", entry.GetEventIpAddress, NpgsqlTypes.NpgsqlDbType.Varchar);
            cmd.Parameters.AddWithValueOrTypeIfNull($"query_id_accepted{i}", entry.AcceptedQueryId, NpgsqlTypes.NpgsqlDbType.Uuid);
            cmd.Parameters.AddWithValueOrTypeIfNull($"query_id_executed{i}", entry.ExecutedQueryId, NpgsqlTypes.NpgsqlDbType.Uuid);
            cmd.Parameters.AddWithValueOrTypeIfNull($"query_id_completed{i}", entry.CompletedQueryId, NpgsqlTypes.NpgsqlDbType.Uuid);
            cmd.Parameters.AddWithValueOrTypeIfNull($"completed_rows{i}", entry.CompletedQueryRows, NpgsqlTypes.NpgsqlDbType.Bigint);
        }

        cmd.CommandText = sqlCommand.ToString();
        return cmd;
    }
}

public static class NpgsqlParameterExtensions
{
    public static void AddWithValueOrTypeIfNull(
        this NpgsqlParameterCollection parameters,
        string parameterName,
        object? value,
        NpgsqlTypes.NpgsqlDbType npgsqlDbType)
    {
        parameters.Add(new NpgsqlParameter(parameterName, npgsqlDbType) { Value = value ?? DBNull.Value });
    }
}