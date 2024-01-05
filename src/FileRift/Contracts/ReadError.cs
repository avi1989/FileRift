namespace FileRift.Contracts;

public class ReadError
{
    public ReadError(int rowNumber, string row, Exception exception)
    {
        RowNumber = rowNumber;
        Row = row;
        Exception = exception;
    }

    public int RowNumber { get; }

    public string Row { get; }

    public Exception Exception { get; }
}