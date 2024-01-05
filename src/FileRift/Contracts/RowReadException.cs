using System.Runtime.Serialization;

namespace FileRift.Contracts;

public class RowReadException : Exception
{
    public RowReadException(int rowNumber, string row, Exception? innerException)
        : base($"Unable to read {rowNumber}. Contents of row are \n{row}\n", innerException)
    {
        this.Row = row;
        this.RowNumber = rowNumber;
    }

    public RowReadException(int rowNumber, string row, string message, Exception? innerException)
        : base(message, innerException)
    {
        this.Row = row;
        this.RowNumber = rowNumber;
    }

    public int RowNumber { get; }

    public string Row { get; }
}