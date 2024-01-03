using System.Data;
using FileRift.Contracts;
using FileRift.Mappers;

namespace FileRift.FixedWidth;

public class FixedWidthFileReader<T> : TypedFileReader<T> where T : class, new()
{
    public FixedWidthFileReader(
        string fileName,
        List<FixedWidthColumnInfo> columns,
        ClassMap<T> map,
        IEnumerable<string>? allowedDateFormats = null)
        : base(new FixedWidthFileDataReader(
                fileName,
                columns,
                allowedDateFormats),
            map)
    {
        
    }

    public FixedWidthFileReader(IDataReader reader, ClassMap<T> map) : base(reader, map)
    {
    }
}