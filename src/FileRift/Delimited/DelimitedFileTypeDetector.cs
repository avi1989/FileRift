namespace FileRift.Delimited;

public class DelimitedFileTypeDetector
{
    public DelimitedFileSettings? GetFileSettings(string[] rows)
    {
        throw new NotImplementedException();
    }
}

public record DelimitedFileSettings(char Separator, char? EscapeCharacter);