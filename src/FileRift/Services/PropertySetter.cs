using System.Globalization;
using System.Reflection;

namespace FileRift.Services;

internal class PropertySetter<T> where T : class
{
    private readonly Dictionary<string, PropertyInfo> _properties = new();

    public PropertySetter()
    {
        var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var property in properties)
        {
            _properties.Add(property.Name, property);
        }
    }

    public void SetValue<TValue>(T data, string propertyName, TValue value)
    {
        var prop = _properties[propertyName];

        if (prop.PropertyType == typeof(TValue))
        {
            prop?.SetValue(data, value);
        }
        else
        {
            var typeToConvert = prop.PropertyType;
            var result = Convert.ChangeType(value, typeToConvert, CultureInfo.CurrentCulture);
            prop?.SetValue(data, result);
        }
    }
}