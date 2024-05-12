using Npgsql;

namespace log_to_db_parsing;

internal static class SQLCommandInserter
{
    public static void InsertIntoDatabase(
        this IEnumerable<NpgsqlCommand> sqlCommands,
        NpgsqlConnection conn)
    {
        foreach (var cmd in sqlCommands)
        {
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();
            cmd.Dispose();
        }
    }
}