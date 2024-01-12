using FileRift.Contracts;
using FileRift.Services;

namespace FileRift.Delimited;

public class DelimitedFileTypeDetector(
    IDelimiterExtractor delimiterExtractor,
    IEscapeCharacterExtractor escapeCharacterExtractor)
{
    public DelimitedFileTypeDetector()
        : this(
            new DelimiterExtractor(),
            new EscapeCharacterExtractor())
    {
    }
    
    public DelimitedFileSettings? GetFileSettings(MemoryStream memoryStream, int rowsToRead)
    {
        return this.GetFileSettings(new StreamReader(memoryStream), rowsToRead);
    }
    
    public DelimitedFileSettings? GetFileSettings(StreamReader reader, int rowsToRead)
    {
        int rowsRead = 0;

        List<string> rows = new List<string>();

        string? line;
        while ((line = reader.ReadLine()) != null && rowsRead < rowsToRead)
        {
            rows.Add(line);
            rowsRead++;
        }

        var rowsArray = rows.ToArray();

        return GetFileSettings(rowsArray);
    }

    public DelimitedFileSettings? GetFileSettings(string[] rows)
    {
        var escapeCharacter = escapeCharacterExtractor.GetEscapeCharacter(rows);
        var delimiter = delimiterExtractor.GetDelimiter(rows);

        return delimiter == null
            ? null
            : new DelimitedFileSettings(delimiter.Value, escapeCharacter);
    }
}

public record DelimitedFileSettings(char Delimiter, char? QuoteField);