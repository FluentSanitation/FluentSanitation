using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using FluentSanitation.Extensions;
using FluentSanitation.Internal;
using Binder = FluentSanitation.Internal.Binder;

namespace FluentSanitation
{
  /// <summary>
  /// The base class for object sanitizers.
  /// </summary>
  /// <typeparam name="T">The type of the object being sanitized</typeparam>
  public abstract class Sanitizer<T> : ISanitizer<T>
  {
    private Dictionary<string, MemberInfo>? _memberCache;
    private readonly Dictionary<string, Action<T, object>> _setterCache
      = new Dictionary<string, Action<T, object>>(StringComparer.OrdinalIgnoreCase);

    private IBinder? _binder;

    protected Dictionary<string, List<SanitizeRule<T>>> Rules { get; }
      = new Dictionary<string, List<SanitizeRule<T>>>();

    object? ISanitizer.Sanitize(object o) =>
      o is T t ? Sanitize(t) : o;

    /// <summary>
    /// Configure rule(s) for a property.
    /// </summary>
    /// <param name="selector">An expression selecting the property to sanitize</param>
    /// <typeparam name="TProperty">The type of the property being sanitized</typeparam>
    /// <returns>A <c>RuleBuilder</c> instance to configure sanitizers</returns>
    public RuleBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> selector) =>
      new RuleBuilder<T, TProperty>(selector, this);

    /// <summary>
    /// Configure rule(s) for a property, inline.
    /// </summary>
    /// <param name="selector">An expression selecting the property to sanitize</param>
    /// <param name="sanitizer">A property sanitizer</param>
    /// <typeparam name="TProperty">The type of the property being sanitized</typeparam>
    /// <returns>A <c>RuleBuilder</c> instance to configure additional sanitizers</returns>
    public RuleBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> selector,
      Func<T, TProperty> sanitizer) =>
      new RuleBuilder<T, TProperty>(selector, this).SetSanitizer(sanitizer);

    /// <summary>
    /// Sanitize an object, using the pre-configured rules.
    /// </summary>
    /// <param name="instance">The object to sanitize</param>
    /// <returns>The sanitized object</returns>
    public virtual T Sanitize(T instance)
    {
      if (instance is null)
        return instance;

      SanitizeInternal(instance);

      return instance;
    }

    internal virtual Sanitizer<T> AddRule(string name, Func<Sanitizer<T>, T, object> action)
    {
      var rule = new SanitizeRule<T>
      {
        Action = action,
        PropertyName = name
      };

      if (!Rules.ContainsKey(rule.PropertyName))
        Rules[rule.PropertyName] = new List<SanitizeRule<T>>();

      Rules[rule.PropertyName].Add(rule);

      return this;
    }

    protected virtual void SanitizeInternal(T instance)
    {
      foreach (var rule in Rules.Values.SelectMany(list => list))
        SanitizeProperty(instance, rule);
    }

    private void SanitizeProperty(T instance, SanitizeRule<T> action)
    {
      var value = action.Action(this, instance);

      if (_setterCache.TryGetValue(action.PropertyName, out var setter))
      {
        setter(instance, value);
        return;
      }

      _memberCache ??= (_binder ??= new Binder()).GetMembers(typeof(T));

      if (!_memberCache.TryGetValue(action.PropertyName, out var member))
        return;

      setter = member switch
      {
        FieldInfo fi => (i, v) => fi?.SetValue(i, v),
        PropertyInfo pi => pi.CreateSetter<T>(),
        _ => throw new InvalidOperationException()
      };

      _setterCache.Add(action.PropertyName, setter);

      setter(instance, value);
    }
  }
}
