using FileRift.Contracts;

namespace FileRift.Services;

public class DelimiterExtractor : IDelimiterExtractor
{
    private readonly HashSet<char> _allowedSeparators;
    private readonly HashSet<char> _commonSeparators = [',', '\t', '|'];

    public DelimiterExtractor(char[] allowedSeparators)
    {
        _allowedSeparators = allowedSeparators.ToHashSet();
    }

    public DelimiterExtractor()
    {
        this._allowedSeparators = new HashSet<char>();
    }

    protected IReadOnlyCollection<char> CommonSeparators => _commonSeparators;

    protected IReadOnlyCollection<char> AllowedSeparators => _allowedSeparators;

    public char? GetDelimiter(string[] rows, char? escapeCharacter = null)
    {
        if (_allowedSeparators.Count != 0)
        {
            return FindSeparator(rows, escapeCharacter, x => AllowedSeparators.Contains(x));
        }

        var separator =
            FindSeparator(rows, escapeCharacter, x => CommonSeparators.Contains(x)) ??
            FindSeparator(rows, escapeCharacter, x => !char.IsLetterOrDigit(x) && x != ' ') ??
            FindSeparator(rows, escapeCharacter, x => x == ' ');

        return separator;
    }

    private static char? FindSeparator(
        string[] rows,
        char? escapeCharacter,
        Func<char, bool> isSeparator)
    {
        Dictionary<int, Dictionary<char, int>> countOfSpecialCharactersPerRow =
            GetCountOfSpecialCharactersPerRow(rows, escapeCharacter, isSeparator);

        var separators = countOfSpecialCharactersPerRow.SelectMany(x => x.Value.Select(y => y.Key))
            .Distinct().ToList();

        char? selectedSeparator = null;
        foreach (var separator in separators)
        {
            if (countOfSpecialCharactersPerRow.Any(
                    x => x.Value.TryGetValue(separator, out var _) == false))
            {
                continue;
            }

            var countOfSeparatorInAllRows =
                countOfSpecialCharactersPerRow.Select(x => x.Value[separator]).ToList().Distinct().Count();

            switch (countOfSeparatorInAllRows)
            {
                case 1 when selectedSeparator == null:
                    selectedSeparator = separator;
                    break;
                case 1:
                case > 1:
                    return null;
            }
        }

        return selectedSeparator;
    }

    private static Dictionary<int, Dictionary<char, int>> GetCountOfSpecialCharactersPerRow(
        string[] rows,
        char? escapeCharacter,
        Func<char, bool> isSeparator)
    {
        Dictionary<int, Dictionary<char, int>> countOfSpecialCharactersPerRow =
            new Dictionary<int, Dictionary<char, int>>();

        for (var i = 0; i < rows.Length; i++)
        {
            var row = rows[i];
            var countOfSpecialCharacterPerRow = new Dictionary<char, int>();
            bool isEscaped = false;
            foreach (var character in row)
            {
                if (character == escapeCharacter)
                {
                    isEscaped = !isEscaped;
                }

                if (isEscaped == false)
                {
                    if (isSeparator(character))
                    {
                        var wasPreviouslyCounted =
                            countOfSpecialCharacterPerRow.TryGetValue(character, out var count);
                        if (wasPreviouslyCounted)
                        {
                            countOfSpecialCharacterPerRow[character] = count + 1;
                        }
                        else
                        {
                            countOfSpecialCharacterPerRow[character] = 1;
                        }
                    }
                }
            }

            countOfSpecialCharactersPerRow[i] = countOfSpecialCharacterPerRow;
        }

        return countOfSpecialCharactersPerRow;
    }
}