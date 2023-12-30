using System.Collections.ObjectModel;

namespace FileRift.Mappers;

public class ColumnMappings : KeyedCollection<string, ColumnMapping>
{
    protected override string GetKeyForItem(ColumnMapping item)
    {
        return item.ColumnName;
    }
}