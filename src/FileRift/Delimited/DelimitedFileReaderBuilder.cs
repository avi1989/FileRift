using FileRift.Mappers;

namespace FileRift.Delimited;

public class DelimitedFileReaderBuilder(string pathToFile)
{
    private char? _delimiter;
    private char? _escapeCharacter = null;
    private bool _hasHeaders = false;
    private bool _shouldAutoTrim = false;
    private string[]? _dateFormats = null;
    private ClassMaps _classMaps = new ClassMaps();

    public DefaultDelimitedFileType Defaults => new DefaultDelimitedFileType(pathToFile);

    public DelimitedFileReaderBuilder WithDelimiter(char delimiter)
    {
        _delimiter = delimiter;
        return this;
    }

    public DelimitedFileReaderBuilder WithEscapeCharacter(char? escape)
    {
        _escapeCharacter = escape;
        return this;
    }

    public DelimitedFileReaderBuilder HasHeaders()
    {
        _hasHeaders = true;
        return this;
    }

    public DelimitedFileReaderBuilder WithTrimmedData()
    {
        _shouldAutoTrim = true;
        return this;
    }

    public DelimitedFileReaderBuilder WithDateFormats(params string[] dateFormats)
    {
        _dateFormats = dateFormats;
        return this;
    }

    public DelimitedFileDataReader BuildDataReader()
    {
        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        return this.BuildDataReader(_hasHeaders, _delimiter.Value, _escapeCharacter, _shouldAutoTrim, _dateFormats);
    }

    public DelimitedFileDataReader BuildDataReader(
        bool hasHeaders,
        char delimiter,
        char? escapeCharacter,
        bool shouldAutoTrim = false,
        IEnumerable<string>? allowedDateFormats = null)
    {
        return new DelimitedFileDataReader(
            pathToFile,
            hasHeaders,
            delimiter,
            escapeCharacter,
            shouldAutoTrim,
            allowedDateFormats);
    }

    public DelimitedFileReader<T> Build<T>(ClassMap<T> classMap) where T : class, new()
    {
        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        if (_hasHeaders == false)
        {
            throw new InvalidOperationException("Cannot build typed data reader without headers");
        }

        return this.Build(_delimiter.Value, _escapeCharacter, classMap, _shouldAutoTrim, _dateFormats);
    }

    public DelimitedFileReader<T> Build<T>() where T : class, new()
    {
        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        if (_hasHeaders == false)
        {
            throw new InvalidOperationException("Cannot build typed data reader without headers");
        }

        var classMap = _classMaps.GetClassMap<T>();

        if (classMap == null)
        {
            throw new InvalidOperationException(
                $"Class map not found for {typeof(T).FullName}. Either register it or invoke build with the class map");
        }

        return this.Build(_delimiter.Value, _escapeCharacter, classMap, _shouldAutoTrim, _dateFormats);
    }


    public DelimitedFileReader<T> Build<T>(
        char delimiter,
        char? escapeCharacter,
        ClassMap<T> classMap,
        bool shouldAutoTrim = false,
        IEnumerable<string>? allowedDateFormats = null) where T : class, new()
    {
        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        return new DelimitedFileReader<T>(
            pathToFile,
            true,
            delimiter,
            escapeCharacter,
            classMap,
            shouldAutoTrim,
            allowedDateFormats);
    }

    public class DefaultDelimitedFileType(string pathToFile)
    {
        public DelimitedFileDataReader BuildCsvDataReader(bool hasHeaders)
        {
            return new DelimitedFileDataReader(pathToFile, hasHeaders, ',', '\"', true);
        }

        public DelimitedFileDataReader BuildPsvDataReader(bool hasHeaders)
        {
            return new DelimitedFileDataReader(pathToFile, hasHeaders, '|', '\"', true);
        }

        public DelimitedFileDataReader BuildTsvDataReader(bool hasHeaders)
        {
            return new DelimitedFileDataReader(pathToFile, hasHeaders, '|', null, true);
        }
    }
}