
using log_to_db_ef;

internal static class LogEntryInserter
{
    public static void InsertLogEntries(this IEnumerable<LogEntry[]> batches, LogDbContext db)
    {
        foreach (var batch in batches)
        {
            try
            {
                db.LogEntries.AddRange(batch);
                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to insert batch. Error: {ex.Message}");
            }
        }
    }

}
