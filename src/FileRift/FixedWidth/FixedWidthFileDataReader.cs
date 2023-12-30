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
            new FixedWithRowSplitter(columnLengths))
    {
    }

    public FixedWidthFileDataReader(
        string fileName,
        IEnumerable<FixedWidthColumnInfo> columns,
        IEnumerable<string>? allowedDateFormats = null) :
        base(new StreamReader(fileName), allowedDateFormats)
    {
        this.Headers = new List<string>();

        var orderedColumns = columns.OrderBy(x => x.Position).ToList();

        var lengths = new int[orderedColumns.Count];
        for (var i = 0; i < orderedColumns.Count; i++)
        {
            var column = orderedColumns[i];
            this.Headers.Add(column.Name);
            lengths[i] = column.Length;
        }

        this.RowSplitter = new FixedWithRowSplitter(lengths);
    }


}