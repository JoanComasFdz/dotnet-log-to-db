namespace log_to_db_performance_functional.Tests;

public class LogLineValidatorTest
{
    [Theory]
    [InlineData("", true)]
    [InlineData(" ", true)]
    [InlineData("a", true)]
    [InlineData("1", true)]
    [InlineData("/", true)]
    [InlineData("2024-12-13T12:34:56.789", false)]
    public void WhereLineIsRelevant_Line_IsSkipped(string line, bool isSkipped)
    {
        var result = LogLineValidator.WhereLineIsRelevant([line]);

        Assert.Equal(!isSkipped, result.Any());
    }
}