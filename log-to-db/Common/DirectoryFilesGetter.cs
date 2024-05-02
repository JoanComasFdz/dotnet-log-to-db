namespace Common;
public static class DirectoryFilesGetter
{
    public static string[] GetFiles(string directoryPath, string filter)
    {
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"The specified directory does not exist: {directoryPath}");
            return [];
        }

        Console.WriteLine($"Searching for {filter} files in directory: {directoryPath}");

        try
        {
            var logFiles = Directory.GetFiles(directoryPath, filter, SearchOption.AllDirectories);
            return logFiles;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while searching the directory '{directoryPath}':{Environment.NewLine}{ex.Message}");
            return [];
        }
    }
}
