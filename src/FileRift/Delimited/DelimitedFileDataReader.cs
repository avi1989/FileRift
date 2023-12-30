using FileRift.Contracts;

namespace FileRift.Delimited;

public class DelimitedFileDataReader : FileRiftDataReader
{
    public DelimitedFileDataReader(
        string fileName,
        bool hasHeaders,
        char delimiter,
        char? escapeCharacter,
        bool shouldAutoTrim = false,
        IEnumerable<string>? allowedDateFormats = null) :
        base(
            new StreamReader(fileName),
            hasHeaders,
            allowedDateFormats,
            new DelimitedRowSplitter(delimiter, escapeCharacter, shouldAutoTrim))
    {
    }

    internal DelimitedFileDataReader(Stream stream, bool hasHeaders, IRowSplitter rowSplitter,
        IEnumerable<string>? allowedDateFormats = null)
        : base(new StreamReader(stream), hasHeaders, allowedDateFormats, rowSplitter)
    {
    }
}