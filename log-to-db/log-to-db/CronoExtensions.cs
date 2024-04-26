internal static class CronoExtensions
{
    public static string ToEasyTime(this TimeSpan ts)
    {
        // Calculate hours and minutes normally
        int hours = ts.Hours;
        int minutes = ts.Minutes;

        // Decide on the format for seconds based on the presence of hours or minutes
        string secondsFormat = (hours > 0 || minutes > 0) ? $"{ts.Seconds}s" : $"{ts.Seconds + ts.Milliseconds / 1000.0:F1}s";

        // Construct the formatted time string
        string formattedTime = $"{(hours > 0 ? $"{hours}h " : "")}" +
                               $"{(hours > 0 || minutes > 0 ? $"{minutes}min " : "")}" +
                               secondsFormat;

        return formattedTime;
    }
}