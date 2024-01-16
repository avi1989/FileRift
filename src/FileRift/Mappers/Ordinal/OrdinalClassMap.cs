using System.Linq.Expressions;
using System.Reflection;
using FileRift.Attributes;
using FileRift.Contracts;

namespace FileRift.Mappers;

public class OrdinalClassMap<T> : IClassMap<T>
{
    private readonly OrdinalColumnMappings _columnMappings;

    public OrdinalClassMap()
    {
        this.Type = typeof(T);

        var properties = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        Properties = new();
        _columnMappings = new();


        foreach (var propertyInfo in properties)
        {
            Properties.Add(propertyInfo.Name, propertyInfo);
            // var columnNameAttribute = propertyInfo.GetCustomAttribute<ColumnNameAttribute>();
            // if (columnNameAttribute != null)
            // {
            //     _columnMappings.Add(
            //         new OrdinalColumnMapping(
            //             columnNameAttribute.ColumnName,
            //             propertyInfo.Name,
            //             propertyInfo.PropertyType));
            // }
        }
    }

    public Dictionary<string, PropertyInfo> Properties { get; set; }

    public Type Type { get; }

    public OrdinalColumnMappings ColumnMappings => _columnMappings;

    public OrdinalClassMap<T> AddColumnMap(int ordinal, Expression<Func<T, object?>> expression)
    {
        var propertyName = GetPropertyName(expression);
        var propertyType = GetPropertyType(expression);
        return AddColumnMap(ordinal, propertyName, propertyType);
    }

    public OrdinalClassMap<T> AddColumnMap(int ordinal, string propertyName, Type propertyType)
    {
        _columnMappings.Add(new(ordinal, propertyName, propertyType));
        return this;
    }

    public string GetPropertyName<T1>(Expression<Func<T1, object?>> expression)
    {
        MemberInfo memberInfo = IClassMap.GetProperty(expression);
        var property = this.Properties[memberInfo.Name];
        return property.Name;
    }

    public Type GetPropertyType<T1>(Expression<Func<T1, object?>> expression)
    {
        var propertyName = this.GetPropertyName(expression);
        var property = this.Properties[propertyName];
        return property.PropertyType;
    }
}