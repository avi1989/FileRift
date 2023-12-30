using FileRift.Contracts;

namespace FileRift.Delimited;

public class DelimitedRowSplitter(char delimiter, char? escapeCharacter, bool shouldTrimSpaces = false)
    : IRowSplitter
{
    public string[] SplitRow(string input)
    {
        int start = 0;
        bool isEscaped = false;
        var result = new List<string>();

        for (var i = 0; i < input.Length; i++)
        {
            if (input[i] == escapeCharacter)
            {
                isEscaped = !isEscaped;
            }

            if (!isEscaped && (input[i] == '\r' || input[i] == '\n' || input[i] == delimiter))
            {
                var col = input[start..(i)];
                if (shouldTrimSpaces)
                {
                    col = col.Trim();
                }

                var colString = col.ToString();

                if (escapeCharacter != null)
                {
                    colString = colString.Replace(escapeCharacter.ToString()!, "");
                }

                result.Add(colString);

                start = i + 1;

                if (input[i] == '\r' || input[i] == '\n')
                {
                    break;
                }
            }
            else if (i == input.Length - 1)
            {
                var col = input[start..(i + 1)];
                if (shouldTrimSpaces)
                {
                    col = col.Trim();
                }

                var colString = col.ToString();
                if (escapeCharacter != null)
                {
                    colString = colString.Replace(escapeCharacter.ToString()!, "");
                }

                result.Add(colString);
                start = i + 1;
            }
        }

        return result.ToArray();
    }
}