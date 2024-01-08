using FileRift.Contracts;

namespace FileRift.FixedWidth;

public class FixedWidthFileDataReader : FileRiftDataReader
{
    public FixedWidthFileDataReader(
        string fileName,
        int[] columnLengths,
        IEnumerable<string>? allowedDateFormats = null) :
        base(
            new StreamReader(fileName),
            false,
            allowedDateFormats,
            new FixedWithRowSplitter(columnLengths),
            null)
    {
    }

    public FixedWidthFileDataReader(
        string fileName,
        ICollection<FixedWidthColumnInfo> columns,
        IEnumerable<string>? allowedDateFormats = null) :
        this(fileName,
             columns.OrderBy(x => x.Position).Select(x => x.Length).ToArray(),
             allowedDateFormats)
    {
        if (columns.All(x => x.Name != null))
        {
            this.Headers = columns.OrderBy(x => x.Position).Select(x => x.Name).ToList();
        }
    }
}