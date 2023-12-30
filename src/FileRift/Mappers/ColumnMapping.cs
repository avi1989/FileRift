﻿namespace FileRift.Mappers;

public class ColumnMapping(string columnName, string propertyName, string dataType)
{
    public string ColumnName { get; } = columnName;

    public string PropertyName { get; } = propertyName;

    public string DataType { get; } = dataType;

    protected bool Equals(ColumnMapping other)
    {
        return ColumnName == other.ColumnName && PropertyName == other.PropertyName && DataType == other.DataType;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ColumnMapping)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ColumnName, PropertyName, DataType);
    }
}