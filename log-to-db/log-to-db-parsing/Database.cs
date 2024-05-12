using Npgsql;

namespace log_to_db_parsing;

internal static class Database
{
    private static readonly string masterConnectionString = "Host=127.0.0.1; Username=postgres; Password=postgres;";

    private static string ConnectionString => masterConnectionString + " Database=logscannerdotnet";

    public static void EnsureCreated()
    {
        using var nasterConn = new NpgsqlConnection(masterConnectionString);
        nasterConn.Open();

        using var cmdSelectDB = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname='logscannerdotnet'", nasterConn);
        var exists = cmdSelectDB.ExecuteScalar() != null;
        if (exists)
        {
            Console.WriteLine("Database exists.");
            return;
        }

        Console.WriteLine("Database does not exist. Creating...");
        CreateDatabase(nasterConn);
        Console.WriteLine("Database created.");

        Console.WriteLine("Creating log table...");
        using var createTableConn = new NpgsqlConnection(ConnectionString);
        createTableConn.Open();
        CreateTable(createTableConn);
        Console.WriteLine("Table created.");
    }

    private static void CreateDatabase(NpgsqlConnection conn)
    {
        using var cmdCreateDB = new NpgsqlCommand("CREATE DATABASE logscannerdotnet", conn);
        cmdCreateDB.ExecuteNonQuery();
    }

    private static void CreateTable(NpgsqlConnection conn)
    {
        using var cmdCreateTable = new NpgsqlCommand();
        cmdCreateTable.Connection = conn;
        cmdCreateTable.CommandText = @"
            CREATE TABLE log
            (
                id SERIAL PRIMARY KEY,
                date timestamp,
                thread varchar(255),
                level varchar(8),
                resource varchar(255),
                file_name varchar(500),
                data TEXT,
                ip_address varchar(25),           -- IP address extracted from GET requests
                query_id_accepted uuid,           -- UUID for accepted queries
                query_id_executed uuid,           -- UUID for executed queries
                query_id_completed uuid,          -- UUID for completed queries
                completed_rows BIGINT                -- Number of rows returned from a query
            );";
        cmdCreateTable.ExecuteNonQuery();

        using var cmdCreateIndexes = new NpgsqlCommand(@"
               CREATE INDEX IF NOT EXISTS idx_file_name ON log (file_name);
               CREATE INDEX IF NOT EXISTS idx_timestamp ON log (date);
               CREATE INDEX IF NOT EXISTS idx_thread_id ON log (thread);
               CREATE INDEX IF NOT EXISTS idx_log_level ON log (level);",
               conn);
        cmdCreateIndexes.ExecuteNonQuery();
    }

    public static NpgsqlConnection OpenConnection()
    {
        var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    public static int GetLogEntriesCount(string logFilePath)
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        var fileName = Path.GetFileName(logFilePath);
        using var cmd = new NpgsqlCommand($"SELECT COUNT(*) FROM log WHERE file_name = '{fileName}'", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public static int RemoveLogEntriesForFile(string logFilePath)
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        var fileName = Path.GetFileName(logFilePath);
        using var cmd = new NpgsqlCommand($"DELETE FROM log WHERE file_name = '{fileName}'", conn);
        return Convert.ToInt32(cmd.ExecuteNonQuery());
    }
}