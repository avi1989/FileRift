using FileRift.Contracts;

namespace FileRift.FixedWidth;

public class FixedWithRowSplitter : IRowSplitter
{
    private readonly List<int> _fieldStartingIndex = new List<int>();

    public FixedWithRowSplitter(int[] columnLengths)
    {
        _fieldStartingIndex.Add(1);
        int position = 1;

        for (var i = 0; i < columnLengths.Length - 1; i++)
        {
            var length = columnLengths[i];
            position += length;
            _fieldStartingIndex.Add(position);
        }
    }

    public string[] SplitRow(string row)
    {
        var result = new List<string>();

        for (int i = 0; i < _fieldStartingIndex.Count; i++)
        {
            var startingPosition = _fieldStartingIndex[i] - 1;
            var endingPosition = i == _fieldStartingIndex.Count - 1
                ? row.Length
                : _fieldStartingIndex[i + 1] - 1;

            var length = endingPosition - startingPosition;

            var substring = row.Substring(startingPosition, length).Trim();
            result.Add(substring);
        }

        return result.ToArray();
    }
}