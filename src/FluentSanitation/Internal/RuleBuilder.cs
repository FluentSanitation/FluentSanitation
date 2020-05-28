using System;
using System.Linq.Expressions;
using FluentSanitation.Extensions;

namespace FluentSanitation.Internal
{
  /// <summary>
  /// Build sanitizer rules for a property.
  /// </summary>
  /// <typeparam name="T">The type of object being sanitized</typeparam>
  /// <typeparam name="TProperty">The type of the property being sanitized</typeparam>
  public class RuleBuilder<T, TProperty>
  {
    private readonly Expression<Func<T, TProperty>> _expression;
    private readonly string _key;
    private readonly Sanitizer<T> _sanitizer;

    private Func<T, TProperty>? _property;

    /// <summary>
    /// Create a <c>RuleBuilder</c>.
    /// </summary>
    /// <param name="selector">An expression selecting the property to sanitize</param>
    /// <param name="sanitizer">The parent <c>Sanitizer</c> that will execute the rules</param>
    public RuleBuilder(Expression<Func<T, TProperty>> selector, Sanitizer<T> sanitizer)
    {
      _expression = selector;
      _key = PropertyName.For(selector);
      _sanitizer = sanitizer;
    }

    /// <summary>
    /// Add sanitation for a property.
    /// </summary>
    /// <param name="sanitizer">The sanitation logic</param>
    /// <returns>This <c>RuleBuilder</c> instance to configure additional sanitizers</returns>
    public RuleBuilder<T, TProperty> SetSanitizer(Func<T, TProperty> sanitizer)
    {
      _sanitizer.AddRule(_key, (s, t) => sanitizer(t)!);

      return this;
    }

    /// <summary>
    /// Add sanitation for a property.
    /// </summary>
    /// <param name="sanitizer">The sanitation logic</param>
    /// <returns>This <c>RuleBuilder</c> instance to configure additional sanitizers</returns>
    public RuleBuilder<T, TProperty> SetSanitizer(Func<TProperty, TProperty> sanitizer)
    {
      _property ??= _expression.Compile();

      _sanitizer.AddRule(_key, (s, t) => sanitizer(_property.Invoke(t))!);

      return this;
    }
  }
}
