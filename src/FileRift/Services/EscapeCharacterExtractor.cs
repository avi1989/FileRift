using System.Text.RegularExpressions;

namespace FileRift.Services;

public class EscapeCharacterExtractor
{
    // Regular expression to match content between quotes
    private static readonly Regex SeparatorPattern = new Regex(@"""([^""]*)""|'([^']*)'");

    public char? GetEscapeCharacter(string[] rows)
    {
        // For each row, find potential separators
        foreach (string row in rows)
        {
            var match = SeparatorPattern.Match(row);
            // If a match is found, return the separator character (either ' or ")
            if (match.Success)
            {
                return row[match.Index]; // Match character index should be the escape character
            }
        }

        // If no separator is found, return null
        return null;
    }
}