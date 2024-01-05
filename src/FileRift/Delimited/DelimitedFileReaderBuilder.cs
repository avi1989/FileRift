using FileRift.Mappers;

namespace FileRift.Delimited;

public class DelimitedFileReaderBuilder(string pathToFile)
{
    private char? _delimiter;
    private char? _escapeCharacter;
    private bool _hasHeaders;
    private bool _shouldAutoTrim;
    private string[]? _dateFormats;
    private readonly ClassMaps _classMaps = new ClassMaps();
    private bool _nullsInsteadOfBlanks = false;

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

    public DelimitedFileReaderBuilder WithNullsInsteadOfBlanks()
    {
        _nullsInsteadOfBlanks = true;
        return this;
    }

    public DelimitedFileReaderBuilder AutoConfigure(int rowsToRead = 20)
    {
        using var streamReader = new StreamReader(pathToFile);
        var fileTypeDetector = new DelimitedFileTypeDetector();
        var config = fileTypeDetector.GetFileSettings(streamReader, rowsToRead);

        if (config == null)
        {
            throw new InvalidOperationException(
                $"Unable to autoconfigure data for file {pathToFile}");
        }

        this._delimiter = config.Delimiter;
        this._escapeCharacter = config.EscapeCharacter;
        return this;
    }

    public DelimitedFileDataReader BuildDataReader()
    {
        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        return BuildDataReader(
            pathToFile,
            _hasHeaders,
            _delimiter.Value,
            _escapeCharacter,
            _shouldAutoTrim,
            _nullsInsteadOfBlanks,
            _dateFormats);
    }


    public static DelimitedFileDataReader BuildDataReader(
        string pathToFile,
        bool hasHeaders,
        char delimiter,
        char? escapeCharacter,
        bool shouldAutoTrim = false,
        bool shouldConvertWhitespaceToNulls = false,
        IEnumerable<string>? allowedDateFormats = null)
    {
        return new DelimitedFileDataReader(
            pathToFile,
            hasHeaders,
            delimiter,
            escapeCharacter,
            shouldAutoTrim,
            shouldConvertWhitespaceToNulls,
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

        return Build(_delimiter.Value, _escapeCharacter, classMap, _shouldAutoTrim, _nullsInsteadOfBlanks, _dateFormats);
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

        return Build(_delimiter.Value, _escapeCharacter, classMap, _shouldAutoTrim, _nullsInsteadOfBlanks, _dateFormats);
    }


    public DelimitedFileReader<T> Build<T>(
        char delimiter,
        char? escapeCharacter,
        ClassMap<T> classMap,
        bool shouldAutoTrim = false,
        bool shouldConvertWhitespaceToNulls = false,
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
            shouldConvertWhitespaceToNulls,
            default,
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