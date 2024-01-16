using FileRift.Mappers;

namespace FileRift.Delimited;

public class DelimitedFileBuilder
{
    private char? _delimiter;
    private char? _quoteField;
    private bool _hasHeaders;
    private bool _shouldIgnoreErrors;
    private bool _shouldIgnoreCase = true;
    private bool _shouldIgnoreSpecialCharacters = true;
    private readonly string _filePath;
    public string[]? _allowedDateFormats;
    private readonly ClassMaps _classMaps = new ClassMaps();

    public DelimitedFileBuilder(string filePath)
    {
        _filePath = filePath;
    }

    public DelimitedFileBuilder(
        string filePath,
        char delimiter,
        char? quoteField) : this(filePath)
    {
        _delimiter = delimiter;
        _quoteField = quoteField;
    }

    public DelimitedFileBuilder HasHeader()
    {
        _hasHeaders = true;
        return this;
    }

    public DelimitedFileBuilder HasHeaders() => HasHeader();

    public DelimitedFileBuilder WithDelimiter(char delimiter)
    {
        _delimiter = delimiter;
        return this;
    }

    public DelimitedFileBuilder WithQuote(char quote)
    {
        _quoteField = quote;
        return this;
    }

    public DelimitedFileBuilder AutoConfigure(int rowsToRead = 20)
    {
        _hasHeaders = true;
        
        using var streamReader = new StreamReader(_filePath);
        var fileTypeDetector = new DelimitedFileTypeDetector();
        var config = fileTypeDetector.GetFileSettings(streamReader, rowsToRead);

        if (config == null)
        {
            throw new InvalidOperationException(
                $"Unable to autoconfigure data for file {_filePath}");
        }

        this._delimiter = config.Delimiter;
        this._quoteField = config.QuoteField;
        return this;
    }

    public DelimitedFileBuilder WithoutExceptions()
    {
        _shouldIgnoreErrors = true;
        return this;
    }

    public DelimitedFileBuilder WithDateFormats(params string[] dateFormats)
    {
        _allowedDateFormats = dateFormats;
        return this;
    }

    public DelimitedFileBuilder WithCaseSensitiveColumns()
    {
        _shouldIgnoreCase = false;
        return this;
    }

    public DelimitedFileBuilder WithExactColumnMatch()
    {
        _shouldIgnoreSpecialCharacters = false;
        _shouldIgnoreCase = true;
        return this;
    }

    public DelimitedFileDataReader BuildDataReader()
    {
        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        return new DelimitedFileDataReader(
            _filePath,
            _hasHeaders,
            _delimiter.Value,
            _quoteField,
            true,
            true,
            _allowedDateFormats);
    }

    public DelimitedFileReader<T> Build<T>(ClassMap<T> classMap) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(classMap);

        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        if (!_hasHeaders)
        {
            throw new InvalidOperationException("Cannot build a reader without headers");
        }

        return new DelimitedFileReader<T>(
            _filePath,
            _hasHeaders,
            _delimiter.Value,
            _quoteField,
            classMap,
            true,
            true,
            _shouldIgnoreErrors,
            _allowedDateFormats);
    }
    
    public DelimitedFileReader<T> Build<T>(OrdinalClassMap<T> classMap) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(classMap);

        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        return new DelimitedFileReader<T>(
            _filePath,
            _hasHeaders,
            _delimiter.Value,
            _quoteField,
            classMap,
            true,
            true,
            _shouldIgnoreErrors,
            _allowedDateFormats);
    }

    public DelimitedFileReader<T> Build<T>() where T : class, new()
    {
        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        var classMap = _classMaps.GetClassMap<T>() ??
                       new AutoClassMap<T>(_shouldIgnoreCase, _shouldIgnoreSpecialCharacters);

        return new DelimitedFileReader<T>(
            _filePath,
            _hasHeaders,
            _delimiter.Value,
            _quoteField,
            classMap,
            true,
            true,
            _shouldIgnoreErrors,
            _allowedDateFormats);
    }
}