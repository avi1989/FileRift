using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FileRift.Attributes;
using FileRift.Contracts;

namespace FileRift.Mappers;

public class ClassMap<T> : IClassMap<T>
{
    private ColumnMappings _columnMappings;

    public ClassMap(bool ignoreCase = false)
    {
        var properties = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        if (ignoreCase)
        {
            Properties =
                new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
            _columnMappings = new(StringComparer.InvariantCultureIgnoreCase);
        }
        else
        {
            Properties = new();
            _columnMappings = new();
        }


        foreach (var propertyInfo in properties)
        {
            Properties.Add(propertyInfo.Name, propertyInfo);
            var columnNameAttribute = propertyInfo.GetCustomAttribute<ColumnNameAttribute>();
            if (columnNameAttribute != null)
            {
                _columnMappings.Add(
                    new ColumnMapping(
                        columnNameAttribute.ColumnName,
                        propertyInfo.Name,
                        propertyInfo.PropertyType));
            }
        }
    }

    protected internal Dictionary<string, PropertyInfo> Properties { get; }

    public IReadOnlyCollection<ColumnMapping> SavedColumnMappings
    {
        get => _columnMappings;
        protected init => _columnMappings = new ColumnMappings(value);
    }

    public Type Type => typeof(T);

    public ClassMap<T> AddColumnMap(string columnName, Expression<Func<T, object?>> expression)
    {
        var propertyName = this.GetPropertyName(expression);
        var propertyType = this.GetPropertyType(expression);
        return AddColumnMap(columnName, propertyName, propertyType);
    }

    public ClassMap<T> Add(string columnName, Expression<Func<T, object?>> expression)
    {
        return AddColumnMap(columnName, expression);
    }

    public ClassMap<T> AddColumnMap(string columnName, string propertyName, Type propertyType)
    {
        var existingColumnMap = _columnMappings.FirstOrDefault(x => x.PropertyName == propertyName);
        if (existingColumnMap != null)
        {
            _columnMappings.Remove(existingColumnMap);
        }

        existingColumnMap =
            _columnMappings.FirstOrDefault(x => x.ColumnName == columnName);

        if (existingColumnMap != null)
        {
            _columnMappings.Remove(existingColumnMap);
        }

        var columnMapping = new ColumnMapping(columnName, propertyName, propertyType);
        _columnMappings.Add(columnMapping);
        return this;
    }

    public virtual ColumnMapping? GetColumnMapping(string columnName)
    {
        _columnMappings.TryGetValue(columnName, out var columnMapping);
        return columnMapping;
    }
    
    public virtual ColumnMapping? GetColumnMapping(Expression<Func<T, object?>> expression)
    {
        var propertyName = this.GetPropertyName(expression);
        _columnMappings.TryGetValue(propertyName, out var columnMapping);
        return columnMapping;
    }

    public Type GetPropertyType<T1>(Expression<Func<T1, object?>> expression)
    {
        var propertyName = this.GetPropertyName(expression);
        var property = this.Properties[propertyName];
        return property.PropertyType;
    }

    
    public string GetPropertyName<T>(Expression<Func<T, object?>> expression)
    {
        var member = IClassMap.GetProperty(expression);
        return member.Name;
    }
}