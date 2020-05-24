using System;
using System.Linq.Expressions;

namespace FluentSanitation.Extensions
{
  internal static class PropertyName
  {
    public static string For<T, TProperty>(Expression<Func<T, TProperty>> expression) =>
      GetMemberName(expression.Body);

    private static string GetMemberName(Expression expression)
    {
      var memberExpression = expression is UnaryExpression unaryExpression
        ? unaryExpression.Operand as MemberExpression
        : expression as MemberExpression;

      if (memberExpression is null)
        throw new ArgumentException("Invalid expression");

      return memberExpression.Member.Name;
    }
  }
}
