using FileRift.Mappers;

namespace FileRift.FixedWidth;

public class FixedWidthFileReaderBuilder(string pathToFile)
{
    private int[]? _columnLengths;
    private List<FixedWidthColumnInfo>? _columns;
    private string[] _dateFormats;
    private ClassMaps _classMaps = new ClassMaps();

    public FixedWidthFileReaderBuilder WithColumnLengths(int[] columnLengths)
    {
        _columnLengths = columnLengths;
        return this;
    }

    public FixedWidthFileReaderBuilder WithColumns(List<FixedWidthColumnInfo> columns)
    {
        _columns = columns;
        return this;
    }

    public FixedWidthFileReaderBuilder WithDateFormats(params string[] dateFormats)
    {
        _dateFormats = dateFormats;
        return this;
    }

    public FixedWidthFileDataReader BuildDataReader(
        int[] columnLengths,
        IEnumerable<string>? allowedDateFormats = null)
    {
        return new FixedWidthFileDataReader(pathToFile, columnLengths, allowedDateFormats);
    }

    public FixedWidthFileDataReader BuildDataReader(
        List<FixedWidthColumnInfo> columns,
        IEnumerable<string>? allowedDateFormats = null)
    {
        return new FixedWidthFileDataReader(pathToFile, columns, allowedDateFormats);
    }

    public FixedWidthFileReader<T> Build<T>(ClassMap<T> classMap) where T : class, new()
    {
        if (_columns == null || _columns.Count == 0)
        {
            throw new InvalidOperationException("Cannot create typed reader without column information");
        }

        return new FixedWidthFileReader<T>(pathToFile, _columns, classMap, _dateFormats);
    }

    public FixedWidthFileReader<T> Build<T>() where T : class, new()
    {
        if (_columns == null || _columns.Count == 0)
        {
            throw new InvalidOperationException("Cannot create typed reader without column information");
        }

        var classMap = _classMaps.GetClassMap<T>();

        if (classMap == null)
        {
            throw new InvalidOperationException(
                $"Class map not found for {typeof(T).FullName}. Either register it or invoke build with the class map");
        }

        return new FixedWidthFileReader<T>(pathToFile, _columns, classMap, _dateFormats);
    }

    public FixedWidthFileReader<T> Build<T>(
        List<FixedWidthColumnInfo> columns,
        ClassMap<T> classMap,
        IEnumerable<string>? allowedDateFormats = null) where T : class, new()
    {
        return new FixedWidthFileReader<T>(pathToFile, columns, classMap, allowedDateFormats);
    }
}