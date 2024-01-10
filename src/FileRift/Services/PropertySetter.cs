using System.Diagnostics;
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

        var actualType = Nullable.GetUnderlyingType(prop.PropertyType);
        bool isNullable = actualType != null;
        if (value == null && isNullable)
        {
            // Here we know that the property we are setting is a nullable prop
            // and that we are setting a null to it.
            prop?.SetValue(data, null);
            return;
        }
        else if (!isNullable)
        {
            actualType = prop.PropertyType;
        }
        
        Debug.Assert(actualType != null);

        if (actualType == typeof(TValue))
        {
            prop?.SetValue(data, value);
        }
        else
        {
            object? result;
            if (value == null && actualType.IsPrimitive)
            {
                result = Activator.CreateInstance(actualType);
            }
            else
            {
                result = Convert.ChangeType(value, actualType!, CultureInfo.CurrentCulture);
            }
            prop?.SetValue(data, result);
        }
    }
}