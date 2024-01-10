using System.Collections.ObjectModel;

namespace FileRift.Mappers;

public class ColumnMappings : KeyedCollection<string, ColumnMapping>
{
    public ColumnMappings()
    {
    }
    
    public ColumnMappings(IEnumerable<ColumnMapping> mappings)
    {
        foreach (var mapping in mappings)
        {
            this.Add(mapping);
        }
    }

    public ColumnMappings(IEqualityComparer<string>? comparer) : base(comparer)
    {
    }

    protected override string GetKeyForItem(ColumnMapping item)
    {
        return item.ColumnName;
    }
}