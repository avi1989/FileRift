using FileRift.FixedWidth;

namespace FileRift.Tests.FixedWidth;

public class FixedWidthRowSplitterTests
{
    [Fact]
    public void SplitRow_Should_SplitRowBasedOnPosition()
    {
        var row = "John                Doe                 20   Male";
        var sut = new FixedWithRowSplitter(new[] { 20, 20, 5, 4 });
        var result = sut.SplitRow(row);

        Assert.Equal(4, result.Length);
        Assert.Equal("John", result[0]);
        Assert.Equal("Doe", result[1]);
        Assert.Equal("20", result[2]);
        Assert.Equal("Male", result[3]);
    }
}