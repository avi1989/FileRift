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
        bool shouldConvertWhitespaceToNulls = false,
        IEnumerable<string>? allowedDateFormats = null) :
        base(
            new StreamReader(fileName),
            hasHeaders,
            allowedDateFormats,
            new DelimitedRowSplitter(delimiter,
                                     escapeCharacter,
                                     shouldAutoTrim,
                                     shouldConvertWhitespaceToNulls),
            escapeCharacter)
    {
    }

    internal DelimitedFileDataReader(
        Stream stream,
        bool hasHeaders,
        IRowSplitter rowSplitter,
        char? enclosingCharacter,
        IEnumerable<string>? allowedDateFormats = null
    )
        : base(new StreamReader(stream), hasHeaders, allowedDateFormats, rowSplitter, enclosingCharacter)
    {
    }
}