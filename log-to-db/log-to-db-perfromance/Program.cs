using log_to_db_performance;
using Common;
using System.Diagnostics;

Console.WriteLine("Log to PostgreSQL - Joan Comas, 2024." + Environment.NewLine);

if (args.Length < 1)
{
    Console.WriteLine("Please provide the directory path containing log files as an argument.");
    return;
}
var directotyWithLogFiles = args[0];

// Gather log files to process
Console.WriteLine($"Getting log files from {directotyWithLogFiles}...");
var logFiles = DirectoryFilesGetter.GetFiles(directotyWithLogFiles, "*.log");
if (logFiles.Length == 0)
{
    Console.WriteLine("Exiting.");
    return;
}

Console.WriteLine($"Found {logFiles.Length} log files in {directotyWithLogFiles}.{Environment.NewLine}");

// Make sure the DB is ready
Console.WriteLine("Checking the database...");
Database.EnsureCreated();
using var conn = Database.OpenConnection();

// Run
const int BatchSize = 1000;
string[] batch = new string[BatchSize];
int currentIndex = 0;

var cronoTotal = Stopwatch.StartNew();
foreach (var logFilePath in logFiles)
{
    try
    {
        var logFileName = Path.GetFileName(logFilePath);
        Console.WriteLine($"Processing file: {logFileName}");

        var deletedEntries = Database.RemoveLogEntriesForFile(logFilePath);
        if (deletedEntries > 0)
        {
            Console.WriteLine($"Removed {deletedEntries} entries already stored for for {logFilePath}.");
        }

        var cronoFile = Stopwatch.StartNew();

        using var reader = new StreamReader(logFilePath);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var spanLine = line.AsSpan();
            if (!LogLineValidator.LineStartsWithDateTime(spanLine))
            {
                continue;
            }

            batch[currentIndex++] = line;
            if (currentIndex == BatchSize)
            {
                LogFileInserter.InsertLogEntries(logFileName, conn, batch, currentIndex);
                currentIndex = 0;
            }
        }

        if (currentIndex > 0)
        {
            LogFileInserter.InsertLogEntries(logFileName, conn, batch, currentIndex);
        }

        cronoFile.Stop();
        var insertedRows = Database.GetLogEntriesCount(logFilePath);
        Console.WriteLine($"Inserted {insertedRows} rows from {Path.GetFileName(logFilePath)} in {cronoFile.Elapsed.ToEasyTime()}.{Environment.NewLine}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to process {logFilePath}. Error: {ex.Message}");
    }
}

conn.Close();
cronoTotal.Stop();
Console.WriteLine($"Done! Total time: {cronoTotal.Elapsed.ToEasyTime()}.");
