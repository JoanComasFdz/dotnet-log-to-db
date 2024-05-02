namespace Common;

public static class StreamReaderExtensions
{
    public static IEnumerable<string> StreamLines(this StreamReader streamReader)
    {
        string? line;
        while ((line = streamReader.ReadLine()) != null)
        {
            yield return line;
        }
    }
}