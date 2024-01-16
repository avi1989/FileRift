using System.Collections.ObjectModel;

namespace FileRift.Mappers;

public class OrdinalColumnMappings : KeyedCollection<int, OrdinalColumnMapping>
{
    public OrdinalColumnMappings()
    {
    }

    protected override int GetKeyForItem(OrdinalColumnMapping item)
    {
        return item.ColumnIndex;
    }
}