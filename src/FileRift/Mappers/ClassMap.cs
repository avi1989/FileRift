using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FileRift.Contracts;

namespace FileRift.Mappers;

public class ClassMap<T> : IClassMap
{
    private readonly ColumnMappings _columnMappings;
    private readonly Dictionary<string, PropertyInfo> _properties;

    public ClassMap()
    {
        _columnMappings = new();

        var properties = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        _properties = new();
        foreach (var propertyInfo in properties)
        {
            _properties.Add(propertyInfo.Name, propertyInfo);
        }
    }

    public IReadOnlyCollection<ColumnMapping> ColumnMappings => _columnMappings;

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
        var columnMapping = new ColumnMapping(columnName, propertyName, propertyType);
        _columnMappings.Add(columnMapping);
        return this;
    }

    private Type GetPropertyType(Expression<Func<T, object?>> expression)
    {
        var propertyName = GetPropertyName(expression);
        var propertyInfo = _properties[propertyName];
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