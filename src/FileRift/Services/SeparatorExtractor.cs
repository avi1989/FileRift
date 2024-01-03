namespace FileRift.Services;

public class SeparatorExtractor
{
    public char? GetSeparator(string[] rows, char? escapeCharacter = null)
    {
        HashSet<char> commonSeparators = [',', '\t', '|'];
        var separator = FindSeparator(rows, escapeCharacter, x => commonSeparators.Contains(x)) ??
                        FindSeparator(rows,
                                      escapeCharacter,
                                      x => !char.IsLetterOrDigit(x) && x != ' ') ??
                        FindSeparator(rows, escapeCharacter, x => x == ' ');

        return separator;
    }

    private char? FindSeparator(string[] rows, char? escapeCharacter, Func<char, bool> isSeparator)
    {
        Dictionary<int, Dictionary<char, int>> countOfSpecialCharactersPerRow =
            GetCountOfSpecialCharactersPerRow(rows, escapeCharacter, isSeparator);

        var separators = countOfSpecialCharactersPerRow.SelectMany(x => x.Value.Select(x => x.Key))
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
                countOfSpecialCharactersPerRow.Select(x => x.Value[separator]).ToList();

            if (countOfSeparatorInAllRows.Distinct().Count() == 1)
            {
                if (selectedSeparator == null)
                {
                    selectedSeparator = separator;
                }
                else
                {
                    return null;
                }
            }

            if (countOfSeparatorInAllRows.Distinct().Count() > 1)
            {
                return null;
            }
        }

        return selectedSeparator;
    }

    private Dictionary<int, Dictionary<char, int>> GetCountOfSpecialCharactersPerRow(
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