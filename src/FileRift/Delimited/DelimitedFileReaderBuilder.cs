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
    private bool _shouldIgnoreErrors;
    private bool _shouldAutoMap = false;
    private bool _shouldIgnoreCase;
    private bool _shouldIgnoreSpecialCharacters;

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

    public DelimitedFileReaderBuilder WithAutoMap(bool ignoreCase, bool ignoreSpecialCharacters)
    {
        _shouldAutoMap = true;
        _shouldIgnoreCase = ignoreCase;
        _shouldIgnoreSpecialCharacters = ignoreSpecialCharacters;
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

    public DelimitedFileReaderBuilder WithoutErrors()
    {
        _shouldIgnoreErrors = true;
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

        if (_shouldAutoMap)
        {
            classMap =
                new AutoClassMap<T>(classMap, _shouldIgnoreCase, _shouldIgnoreSpecialCharacters);
        }

        return Build(
            delimiter: _delimiter.Value,
            escapeCharacter: _escapeCharacter,
            classMap: classMap,
            shouldAutoTrim: _shouldAutoTrim,
            shouldConvertWhitespaceToNulls: _nullsInsteadOfBlanks,
            allowedDateFormats: _dateFormats);
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
            if (_shouldAutoMap)
            {
                classMap = new AutoClassMap<T>(_shouldIgnoreCase, _shouldIgnoreSpecialCharacters);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Class map not found for {typeof(T).FullName}. Either register it or invoke build with the class map");
            }
        }

        return Build(_delimiter.Value,
                     _escapeCharacter,
                     classMap,
                     _shouldAutoTrim,
                     _nullsInsteadOfBlanks,
                     _shouldIgnoreErrors,
                     _dateFormats);
    }


    public DelimitedFileReader<T> Build<T>(
        char delimiter,
        char? escapeCharacter,
        ClassMap<T> classMap,
        bool shouldAutoTrim = false,
        bool shouldConvertWhitespaceToNulls = false,
        bool shouldIgnoreErrors = false,
        IEnumerable<string>? allowedDateFormats = null) where T : class, new()
    {
        if (_delimiter == null)
        {
            throw new InvalidOperationException("Separator not configured");
        }

        return new DelimitedFileReader<T>(
            fileName: pathToFile,
            hasHeader: true,
            delimiter: delimiter,
            escapeCharacter: escapeCharacter,
            map: classMap,
            shouldAutoTrim: shouldAutoTrim,
            shouldConvertWhitespaceToNulls: shouldConvertWhitespaceToNulls,
            shouldIgnoreErrors: shouldIgnoreErrors,
            allowedDateFormats: allowedDateFormats);
    }

    public class DefaultDelimitedFileType(string pathToFile)
    {
        public DelimitedFileReader<T> BuildCsvReader<T>(
            bool ignoreHeaderCase = true,
            bool ignoreSpecialCharactersInHeader = true) where T : class, new()
        {
            var autoMap = new AutoClassMap<T>(ignoreHeaderCase, ignoreSpecialCharactersInHeader);
            return new DelimitedFileReader<T>(
                pathToFile,
                true,
                ',',
                '\"',
                autoMap,
                true,
                true,
                false);
        }

        public DelimitedFileReader<T> BuildAutoConfiguredReader<T>(
            int rowsToReadForConfiguration = 20,
            bool ignoreHeaderCase = true,
            bool ignoreSpecialCharactersInHeader = true) where T : class, new()
        {
            using var streamReader = new StreamReader(pathToFile);
            var fileTypeDetector = new DelimitedFileTypeDetector();
            var config = fileTypeDetector.GetFileSettings(streamReader, rowsToReadForConfiguration);

            if (config == null)
            {
                throw new InvalidOperationException(
                    $"Unable to autoconfigure data for file {pathToFile}");
            }

            var autoMap = new AutoClassMap<T>(ignoreHeaderCase, ignoreSpecialCharactersInHeader);
            return new DelimitedFileReader<T>(
                pathToFile,
                true,
                config.Delimiter,
                config.EscapeCharacter,
                autoMap,
                true,
                true,
                false);
        }

        public DelimitedFileDataReader BuildCsvDataReader(bool hasHeaders)
        {
            return new DelimitedFileDataReader(pathToFile, hasHeaders, ',', '\"', true);
        }
    }
}