namespace log_to_db_performance_functional.Tests;

public class SQLCommandCreatorTest
{
    [Fact]
    public void ToSQLCommands_1BatchIsEmpty_ReturnsEmpty()
    {
        var enumerable = SQLCommandCreator.ToSQLCommands([[]], "filename");
        Assert.Empty(enumerable);
    }

    [Fact]
    public void ToSQLCommands_1Batch1LogEntry_Returns1Command()
    {
        var logEntry = LogEntry.Create(DateTime.Now, "thread-1", "INFO", "Component", "message");

        var enumerable = SQLCommandCreator.ToSQLCommands([[logEntry]], "filename");

        var command = enumerable.First();
        Assert.Equal("INSERT INTO log (file_name, timestamp, thread_id, log_level, component, message) VALUES (@file_name1, @timestamp1, @thread_id1, @log_level1, @component1, @message1)", command.CommandText);
        Assert.Equal("filename", command.Parameters["@file_name1"].Value);
        Assert.Equal(logEntry.Timestamp, command.Parameters["@timestamp1"].Value);
        Assert.Equal(logEntry.ThreadId.ToString(), command.Parameters["@thread_id1"].Value);
        Assert.Equal(logEntry.LogLevel.ToString(), command.Parameters["@log_level1"].Value);
        Assert.Equal(logEntry.Component.ToString(), command.Parameters["@component1"].Value);
        Assert.Equal(logEntry.Message.ToString(), command.Parameters["@message1"].Value);
    }

    [Fact]
    public void ToSQLCommands_1Batch2LogEntry_Returns1Command()
    {
        var logEntry1 = LogEntry.Create(DateTime.Now, "thread-1", "INFO1", "Component1", "message1");
        var logEntry2 = LogEntry.Create(DateTime.Now, "thread-2", "INFO2", "Component2", "message2");

        var enumerable = SQLCommandCreator.ToSQLCommands([[logEntry1, logEntry2]], "filename");

        var command = enumerable.First();
        Assert.Equal("INSERT INTO log (file_name, timestamp, thread_id, log_level, component, message) VALUES (@file_name1, @timestamp1, @thread_id1, @log_level1, @component1, @message1), (@file_name2, @timestamp2, @thread_id2, @log_level2, @component2, @message2)", command.CommandText);
        Assert.Equal("filename", command.Parameters["@file_name1"].Value);
        Assert.Equal(logEntry1.Timestamp, command.Parameters["@timestamp1"].Value);
        Assert.Equal(logEntry1.ThreadId.ToString(), command.Parameters["@thread_id1"].Value);
        Assert.Equal(logEntry1.LogLevel.ToString(), command.Parameters["@log_level1"].Value);
        Assert.Equal(logEntry1.Component.ToString(), command.Parameters["@component1"].Value);
        Assert.Equal(logEntry1.Message.ToString(), command.Parameters["@message1"].Value);
        Assert.Equal("filename", command.Parameters["@file_name2"].Value);
        Assert.Equal(logEntry2.Timestamp, command.Parameters["@timestamp2"].Value);
        Assert.Equal(logEntry2.ThreadId.ToString(), command.Parameters["@thread_id2"].Value);
        Assert.Equal(logEntry2.LogLevel.ToString(), command.Parameters["@log_level2"].Value);
        Assert.Equal(logEntry2.Component.ToString(), command.Parameters["@component2"].Value);
        Assert.Equal(logEntry2.Message.ToString(), command.Parameters["@message2"].Value);
    }


    [Fact]
    public void ToSQLCommands_2Batches1LogEntryEach_Returns2Command()
    {
        var logEntry1 = LogEntry.Create(DateTime.Now, "thread-1", "INFO1", "Component1", "message1");
        var logEntry2 = LogEntry.Create(DateTime.Now, "thread-2", "INFO2", "Component2", "message2");

        var enumerable = SQLCommandCreator.ToSQLCommands([[logEntry1], [logEntry2]], "filename");

        var commands = enumerable.ToArray();
        Assert.Equal(2, commands.Length);

        var command1 = commands[0];
        Assert.Equal("INSERT INTO log (file_name, timestamp, thread_id, log_level, component, message) VALUES (@file_name1, @timestamp1, @thread_id1, @log_level1, @component1, @message1)", command1.CommandText);
        Assert.Equal("filename", command1.Parameters["@file_name1"].Value);
        Assert.Equal(logEntry1.Timestamp, command1.Parameters["@timestamp1"].Value);
        Assert.Equal(logEntry1.ThreadId.ToString(), command1.Parameters["@thread_id1"].Value);
        Assert.Equal(logEntry1.LogLevel.ToString(), command1.Parameters["@log_level1"].Value);
        Assert.Equal(logEntry1.Component.ToString(), command1.Parameters["@component1"].Value);
        Assert.Equal(logEntry1.Message.ToString(), command1.Parameters["@message1"].Value);

        var command2 = commands[1];
        Assert.Equal("INSERT INTO log (file_name, timestamp, thread_id, log_level, component, message) VALUES (@file_name1, @timestamp1, @thread_id1, @log_level1, @component1, @message1)", command2.CommandText);
        Assert.Equal("filename", command2.Parameters["@file_name1"].Value);
        Assert.Equal(logEntry2.Timestamp, command2.Parameters["@timestamp1"].Value);
        Assert.Equal(logEntry2.ThreadId.ToString(), command2.Parameters["@thread_id1"].Value);
        Assert.Equal(logEntry2.LogLevel.ToString(), command2.Parameters["@log_level1"].Value);
        Assert.Equal(logEntry2.Component.ToString(), command2.Parameters["@component1"].Value);
        Assert.Equal(logEntry2.Message.ToString(), command2.Parameters["@message1"].Value);
    }
}