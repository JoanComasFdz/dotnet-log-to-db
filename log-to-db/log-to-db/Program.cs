using log_to_db;
using System.Diagnostics;

Console.WriteLine("Log to PostgreSQL - Joan Comas, 2024." + Environment.NewLine);

var logFiles = GetLogFiles();

Console.WriteLine("Checking the database...");
Database.EnsureCreated();

Console.WriteLine($"Processing {logFiles.Length} log files...");
var cronoTotal = Stopwatch.StartNew();
foreach (var logFilePath in logFiles)
{
    try
    {
        Console.WriteLine($"Processing file: {logFilePath}");

        if (Database.GetLogEntriesCount(logFilePath) > 0)
        {
            var deletedEntries = Database.RemoveLogEntriesForFile(logFilePath);
            Console.WriteLine($"Removed {deletedEntries} entries already stored for for {logFilePath}.");
        }

        var cronoFile = Stopwatch.StartNew();

        LogFileInserter.ProcessLogFile(logFilePath);

        cronoFile.Stop();
        int insertedRows = Database.GetLogEntriesCount(logFilePath);  // Adjust this method if it needs to count rows per file.
        Console.WriteLine($"Inserted {insertedRows} rows from {Path.GetFileName(logFilePath)} in {cronoFile.Elapsed.ToEasyTime()}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to process {logFilePath}. Error: {ex.Message}");
    }
}
cronoTotal.Stop();
Console.WriteLine($"Total time: {cronoTotal.Elapsed.ToEasyTime()}.");

static string[] GetLogFiles()
{
    string[] args = Environment.GetCommandLineArgs();
    if (args.Length <= 1)
    {
        Console.WriteLine("Please provide the directory path containing log files as an argument.");
        return [];
    }

    string directoryPath = args[1];
    if (!Directory.Exists(directoryPath))
    {
        Console.WriteLine($"The specified directory does not exist: {directoryPath}");
        return [];
    }

    Console.WriteLine($"Searching for .log files in directory: {directoryPath}");

    try
    {
        var logFiles = Directory.GetFiles(directoryPath, "*.log", SearchOption.AllDirectories);
        if (logFiles.Length == 0)
        {
            Console.WriteLine("No .log files found in the specified directory.");
            return [];
        }
        return logFiles;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while searching the directory '{directoryPath}':{Environment.NewLine}{ex.Message}");
        return [];
    }
}