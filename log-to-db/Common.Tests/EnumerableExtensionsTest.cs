using Common;

namespace Common.Tests;

public class  EnumerableExtensionsTest
{
    record TestItem(DateTime Timestamp, string ThreadId, string LogLevel, string Component, string Message);

    [Fact]
    public void InBatchesOf_1LogEntryAndBatchOf1_Returns1BatchWith1()
    {
        var logEntries = new TestItem[]
        {
            new(DateTime.Now, "1", "INFO", "Component", "Message")
        };

        var batches = logEntries.InBatchesOf(1).ToArray();

        Assert.Single(batches);
        Assert.Single(batches[0]);
        Assert.Equal(logEntries[0], batches[0][0]);
    }

    [Fact]
    public void InBatchesOf_2LogEntriesAndBAtchOf1_Returns2BatchesWIth1LogEntryEach()
    {
        var logEntries = new TestItem[]
        {
            new(DateTime.Now, "1", "INFO", "Component", "Message"),
            new(DateTime.Now, "2", "INFO2", "Component2", "Message2"),
        };

        var batches = logEntries.InBatchesOf(1).ToArray();

        Assert.Equal(2, batches.Length);

        Assert.Single(batches[0]);
        Assert.Equal(logEntries[0], batches[0][0]);
        
        Assert.Single(batches[1]);
        Assert.Equal(logEntries[1], batches[1][0]);
    }

    [Fact]
    public void InBatchesOf_2LogEntriesAndBatchOf2_Returns1BatchWIth2LogEntries()
    {
        var logEntries = new TestItem[]
        {
            new(DateTime.Now, "1", "INFO", "Component", "Message"),
            new(DateTime.Now, "2", "INFO2", "Component2", "Message2"),
        };

        var batches = logEntries.InBatchesOf(2).ToArray();

        Assert.Single(batches);
        Assert.Equal(2, batches[0].Length);
        Assert.Equal(logEntries[0], batches[0][0]);
        Assert.Equal(logEntries[1], batches[0][1]);
    }

    [Fact]
    public void InBatchesOf_3LogEntriesAndBatchOf2_Returns1BatchWIth2And1BatchWith1LogEntry()
    {
        var logEntries = new TestItem[]
        {
            new(DateTime.Now, "1", "INFO", "Component", "Message"),
            new(DateTime.Now, "2", "INFO2", "Component2", "Message2"),
            new(DateTime.Now, "3", "INFO3", "Component3", "Message3"),
        };

        var batches = logEntries.InBatchesOf(2).ToArray();

        Assert.Equal(2, batches.Length);

        Assert.Equal(2, batches[0].Length);
        Assert.Equal(logEntries[0], batches[0][0]);
        Assert.Equal(logEntries[1], batches[0][1]);

        Assert.Single(batches[1]);
        Assert.Equal(logEntries[2], batches[1][0]);
    }
}