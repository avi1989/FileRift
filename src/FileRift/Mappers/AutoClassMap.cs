namespace FileRift.Mappers;

public class AutoClassMap<T>(bool ignoreCase, bool ignoreSpecialCharacters)
    : ClassMap<T>(ignoreCase)
{
    public AutoClassMap(ClassMap<T> classMap, bool ignoreCase, bool ignoreSpecialCharacters) : this(
        ignoreCase,
        ignoreSpecialCharacters)
    {
        this.SavedColumnMappings = classMap.SavedColumnMappings;
    }

    public override ColumnMapping? GetColumnMapping(string columnName)
    {
        var savedColumnMapping = base.GetColumnMapping(columnName);
        if (savedColumnMapping != null)
        {
            return savedColumnMapping;
        }

        if (Properties.TryGetValue(columnName, out var propInfo))
        {
            this.AddColumnMap(columnName, propInfo.Name, propInfo.PropertyType);
            return new ColumnMapping(columnName, propInfo.Name, propInfo.PropertyType);
        }

        if (!ignoreSpecialCharacters)
        {
            return null;
        }

        var propertyKeys = Properties.Select(x => x.Key);
        var columnNameWithoutSpecialCharacters =
            columnName.Replace("-", "").Replace("_", "").Replace(" ", "").Trim();

        foreach (var propertyKey in propertyKeys)
        {
            var keyWithoutSpecialCharacters =
                propertyKey.Replace("_", "").Replace(" ", "").Trim();

            var stringComparison = ignoreCase
                ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.InvariantCulture;

            if (string.Equals(columnNameWithoutSpecialCharacters, keyWithoutSpecialCharacters, stringComparison))
            {
                var selectedProperty = Properties[propertyKey];
                this.AddColumnMap(columnName, selectedProperty.Name, selectedProperty.PropertyType);
                return new ColumnMapping(
                    columnName,
                    selectedProperty.Name,
                    selectedProperty.PropertyType);
            }
        }

        return null;
    }
}