using FileRift.Contracts;

namespace FileRift.Delimited;

public class DelimitedRowSplitter(char delimiter, char? escapeCharacter, bool shouldTrimSpaces = false)
    : IRowSplitter
{
    public string[] SplitRow(string row)
    {
        int start = 0;
        bool isEscaped = false;
        var result = new List<string>();

        for (var i = 0; i < row.Length; i++)
        {
            if (row[i] == escapeCharacter)
            {
                isEscaped = !isEscaped;
            }

            if (!isEscaped && (row[i] == '\r' || row[i] == '\n' || row[i] == delimiter))
            {
                var col = row[start..(i)];
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

                if (row[i] == '\r' || row[i] == '\n')
                {
                    break;
                }
            }
            else if (i == row.Length - 1)
            {
                var col = row[start..(i + 1)];
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