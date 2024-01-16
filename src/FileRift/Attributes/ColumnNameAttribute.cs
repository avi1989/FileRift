namespace FileRift.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnNameAttribute(string columnName) : Attribute
{
    public string ColumnName => columnName;
}