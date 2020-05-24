using System;
using System.Linq.Expressions;
using FluentSanitation.Extensions;

namespace FluentSanitation.Internal
{
  public class RuleBuilder<T, TProperty> where T : class
  {
    private readonly Expression<Func<T, TProperty>> _expression;
    private readonly string _key;
    private readonly Sanitizer<T> _sanitizer;

    private Func<T, TProperty>? _property;

    public RuleBuilder(Expression<Func<T, TProperty>> selector, Sanitizer<T> sanitizer)
    {
      _expression = selector;
      _key = PropertyName.For(selector);
      _sanitizer = sanitizer;
    }

    public RuleBuilder<T, TProperty> SetSanitizer(Func<T, TProperty> sanitizer)
    {
      _sanitizer.AddRule(_key, (s, t) => sanitizer(t)!);

      return this;
    }

    public RuleBuilder<T, TProperty> SetSanitizer(Func<TProperty, TProperty> sanitizer)
    {
      _property ??= _expression.Compile();

      _sanitizer.AddRule(_key, (s, t) => sanitizer(_property.Invoke(t))!);

      return this;
    }
  }
}
