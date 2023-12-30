﻿using System.Diagnostics;
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

    public ClassMap<T> AddColumnMap(string columnName, Expression<Func<T, object>> expression)
    {
        var a = GetPropertyName(expression);
        var propertyType = GetPropertyType(expression);

        var columnMapping = new ColumnMapping(columnName, a, propertyType);
        _columnMappings.Add(columnMapping);
        return this;
    }

    private string GetPropertyType(Expression<Func<T, object>> expression)
    {
        var propertyName = GetPropertyName(expression);
        var propertyInfo = _properties[propertyName];
        Debug.Assert(propertyInfo?.PropertyType?.FullName != null, "propertyInfo != null");

        return propertyInfo.PropertyType.FullName;
    }

    private string GetPropertyName(Expression<Func<T, object>> expression)
    {
        MemberExpression memberExpression = null;
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