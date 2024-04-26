using Npgsql;

namespace log_to_db;

internal static class Database
{
    private static readonly string masterConnectionString = "Host=127.0.0.1; Username=postgres; Password=postgres;";

    public static string ConnectionString => masterConnectionString + " Database=logscannerdotnet";

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
            CREATE TABLE IF NOT EXISTS log (
                id SERIAL PRIMARY KEY,
                file_name VARCHAR(255) NOT NULL,
                timestamp TIMESTAMP WITH TIME ZONE NOT NULL,
                thread_id VARCHAR(255) NOT NULL,
                log_level VARCHAR(20) NOT NULL,
                component VARCHAR(255) NOT NULL,
                message TEXT
            )";
        cmdCreateTable.ExecuteNonQuery();

        using var cmdCreateIndexes = new NpgsqlCommand(@"
                CREATE INDEX IF NOT EXISTS idx_file_name ON log (file_name);
                CREATE INDEX IF NOT EXISTS idx_timestamp ON log (timestamp);
                CREATE INDEX IF NOT EXISTS idx_thread_id ON log (thread_id);
                CREATE INDEX IF NOT EXISTS idx_log_level ON log (log_level);",
                conn);
        cmdCreateIndexes.ExecuteNonQuery();
    }

    public static int GetLogEntriesCount()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM log", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}