namespace FileRift.Mappers;

public class OrdinalColumnMapping
{
    public OrdinalColumnMapping(int columnIndex, string propertyName, Type dataType)
    {
        ColumnIndex = columnIndex;
        PropertyName = propertyName;
        DataType = dataType;
    }

    public int ColumnIndex { get; }

    public string PropertyName { get; }

    public Type DataType { get; }
}