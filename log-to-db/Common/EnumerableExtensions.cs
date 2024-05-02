namespace Common;

public static class EnumerableExtensions
{
    public static IEnumerable<T[]> InBatchesOf<T>(this IEnumerable<T> logEntries, short batchSize)
    {
        var batch = new T[batchSize];
        var i = 0;
        foreach (var logEntry in logEntries)
        {
            batch[i++] = logEntry;
            if (i >= batchSize)
            {
                yield return batch.ToArray();
                i = 0;
            }
        }

        if (i > 0)
        {
            yield return batch[0..i].ToArray();
        }
    }
}