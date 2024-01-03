using FileRift.Contracts;
using FileRift.Services;

namespace FileRift.Delimited;

public class DelimitedFileTypeDetector
{
    private IDelimiterExtractor _delimiterExtractor;
    private IEscapeCharacterExtractor _escapeCharacterExtractor;

    public DelimitedFileTypeDetector(
        IDelimiterExtractor delimiterExtractor,
        IEscapeCharacterExtractor escapeCharacterExtractor)
    {
        _delimiterExtractor = delimiterExtractor;
        _escapeCharacterExtractor = escapeCharacterExtractor;
    }

    public DelimitedFileTypeDetector() : this(new DelimiterExtractor(),
                                              new EscapeCharacterExtractor())
    {
    }

    public DelimitedFileSettings? GetFileSettings(string[] rows)
    {
        throw new NotImplementedException();
    }
}

public record DelimitedFileSettings(char Separator, char? EscapeCharacter);