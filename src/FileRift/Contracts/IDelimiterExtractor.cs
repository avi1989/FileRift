namespace FileRift.Contracts;

public interface IDelimiterExtractor
{
    public char? GetDelimiter(string[] rows, char? escapeCharacter = null);
}