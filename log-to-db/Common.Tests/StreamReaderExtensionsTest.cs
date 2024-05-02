using System.Text;

namespace Common.Tests;

public class StreamReaderExtensionsTest
{
    [Fact]
    public void StreamLines_NoLines_ReturnsEmpty()
    {
        var empty = new StreamReader(new MemoryStream());

        var result = empty.StreamLines();

        Assert.Empty(result);
    }

    [Fact]
    public void StreamLines_1Line_Returns1()
    {
        var oneLine = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("1 line")));

        var enumerable = oneLine.StreamLines();

        var result = enumerable.ToArray();
        Assert.Single(result);
        Assert.Equal("1 line", result.Single());
    }

    [Fact]
    public void StreamLines_2Lines_Returns2()
    {
        var twoLines = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes($"1 line{Environment.NewLine}2 lines")));
        
        var enumerable = twoLines.StreamLines();
        
        var result = enumerable.ToArray();
        Assert.Equal(2, result.Length);
        Assert.Equal("1 line", result[0]);
        Assert.Equal("2 lines", result[1]);
    }
}
