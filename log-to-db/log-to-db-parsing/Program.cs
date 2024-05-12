using Common;
using log_to_db_parsing;
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
var cronoTotal = Stopwatch.StartNew();
foreach (var logFilePath in logFiles)
{
    try
    {
        var logFileName = Path.GetFileName(logFilePath);
        Console.WriteLine($"Processing file: {logFileName}"); 
        
        var deletedEntries = Database.RemoveLogEntriesForFile(logFileName);
        if (deletedEntries > 0)
        {
            Console.WriteLine($"Removed {deletedEntries} entries already stored for for {logFileName}.");
        }

        var cronoFile = Stopwatch.StartNew();

        using var logFileStreamReader = new StreamReader(logFilePath);
        logFileStreamReader.StreamLines()
            .WhereLineIsRelevant()
            .ToLogEntry()
            .InBatchesOf(1000)
            .ToSQLCommands(logFileName)
            .InsertIntoDatabase(conn);

        cronoFile.Stop();

        var insertedRows = Database.GetLogEntriesCount(logFileName);
        Console.WriteLine($"Inserted {insertedRows} rows from {logFileName} in {cronoFile.Elapsed.ToEasyTime()}.{Environment.NewLine}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to process {logFilePath}. Error: {ex.Message}");
    }
}
cronoTotal.Stop();
Console.WriteLine($"Done! Total time: {cronoTotal.Elapsed.ToEasyTime()}.");
