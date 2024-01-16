using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FileRift.Attributes;
using FileRift.Contracts;

namespace FileRift.Mappers;

public class ClassMap<T> : IClassMap
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
        var propertyName = GetPropertyName(expression);
        var propertyType = GetPropertyType(expression);
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

    private Type GetPropertyType(Expression<Func<T, object?>> expression)
    {
        var propertyName = GetPropertyName(expression);
        var propertyInfo = Properties[propertyName];
        Debug.Assert(propertyInfo?.PropertyType?.FullName != null, "propertyInfo != null");

        return propertyInfo.PropertyType;
    }

    private static string GetPropertyName(Expression<Func<T, object?>> expression)
    {
        MemberExpression memberExpression;
        if (expression.Body.NodeType == ExpressionType.Convert)
        {
            var unaryExpression = (UnaryExpression)expression.Body;
            memberExpression = (MemberExpression)unaryExpression.Operand;
        }
        else
        {
            memberExpression = (MemberExpression)expression.Body;
        }

        if (memberExpression.Expression?.NodeType == ExpressionType.MemberAccess)
        {
            throw new ArgumentException(
                "The provided expression is a nested member access expression which is not allowed.");
        }

        return memberExpression.Member.Name;
    }
}