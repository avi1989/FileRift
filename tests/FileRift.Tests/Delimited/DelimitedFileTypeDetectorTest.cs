using System.Text;
using FileRift.Contracts;
using FileRift.Delimited;
using NSubstitute;

namespace FileRift.Tests.Delimited;

public class DelimitedFileTypeDetectorTest
{
    [Fact]
    public void DelimitedFileSettings_Should_ReturnDelimitersAndEscapeCharacters()
    {
        var delimiterExtractor = Substitute.For<IDelimiterExtractor>();
        var escapeCharacterExtractor = Substitute.For<IEscapeCharacterExtractor>();

        var input = new string[2];

        var delimiter = '\0';
        char? escapeCharacter = '"';
        delimiterExtractor.GetDelimiter(input).Returns(delimiter);
        escapeCharacterExtractor.GetEscapeCharacter(input).Returns(escapeCharacter);

        var sut = new DelimitedFileTypeDetector(delimiterExtractor, escapeCharacterExtractor);
        var result = sut.GetFileSettings(input);

        Assert.Equal(delimiter, result.Delimiter);
        Assert.Equal(escapeCharacter, result.EscapeCharacter);
    }

    [Fact]
    public void GetFileSettings_ShouldReturnResult_EvenIfEscapeCharacterIsNull()
    {
        var delimiterExtractor = Substitute.For<IDelimiterExtractor>();
        var escapeCharacterExtractor = Substitute.For<IEscapeCharacterExtractor>();

        var input = new string[2];

        var delimiter = '\0';
        char? escapeCharacter = null;
        delimiterExtractor.GetDelimiter(input).Returns(delimiter);
        escapeCharacterExtractor.GetEscapeCharacter(input).Returns(escapeCharacter);

        var sut = new DelimitedFileTypeDetector(delimiterExtractor, escapeCharacterExtractor);
        var result = sut.GetFileSettings(input);

        Assert.Equal(delimiter, result.Delimiter);
        Assert.Equal(escapeCharacter, result.EscapeCharacter);
    }

    [Fact]
    public void GetFileSettings_ShouldReturnNull_If_DelimiterIsNull()
    {
        var delimiterExtractor = Substitute.For<IDelimiterExtractor>();
        var escapeCharacterExtractor = Substitute.For<IEscapeCharacterExtractor>();

        var input = new string[2];

        char? delimiter = null;
        char? escapeCharacter = '"';
        delimiterExtractor.GetDelimiter(input).Returns(delimiter);
        escapeCharacterExtractor.GetEscapeCharacter(input).Returns(escapeCharacter);

        var sut = new DelimitedFileTypeDetector(delimiterExtractor, escapeCharacterExtractor);
        var result = sut.GetFileSettings(input);

        Assert.Null(result);
    }

    [Fact]
    public void GetFileSettings_ShouldReadStreamForCorrectNumberOfRows()
    {
        var delimiterExtractor = Substitute.For<IDelimiterExtractor>();
        var escapeCharacterExtractor = Substitute.For<IEscapeCharacterExtractor>();
        int rowsToRead = 20;
        StringBuilder stringBuilder = new StringBuilder();
        Enumerable.Range(1, 100).ToList()
            .ForEach(x => stringBuilder.AppendLine($"Another,Line,{x}"));
        var memoryStream =
            new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
        char? delimiter = null;
        char? escapeCharacter = '"';
        
        
        delimiterExtractor.GetDelimiter(Arg.Any<string[]>())
            .Returns(delimiter);
        escapeCharacterExtractor.GetEscapeCharacter(Arg.Any<string[]>())
            .Returns(escapeCharacter);

        var sut = new DelimitedFileTypeDetector(delimiterExtractor, escapeCharacterExtractor);
        sut.GetFileSettings(memoryStream, rowsToRead);

        delimiterExtractor.Received()
            .GetDelimiter(Arg.Is<string[]>(x => x.Length == rowsToRead));

        escapeCharacterExtractor.Received()
            .GetEscapeCharacter(Arg.Is<string[]>(x => x.Length == rowsToRead));
    }
}