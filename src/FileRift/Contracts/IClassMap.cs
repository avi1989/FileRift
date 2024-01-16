using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace FileRift.Contracts;

public interface IClassMap
{
    Type Type { get; }
    
    public static MemberInfo GetProperty<T>(Expression<Func<T, object?>> expression)
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

        return memberExpression.Member;
    }
}

public interface IClassMap<T> : IClassMap
{
}