using System;
using System.Reflection;

namespace FluentSanitation.Extensions
{
  internal static class PropertyInfoExtensions
  {
    private static readonly MethodInfo? GenericSetterCreationMethod =
      typeof(PropertyInfoExtensions).GetMethod(nameof(CreateSetterGeneric),
        BindingFlags.Static | BindingFlags.NonPublic);

    public static Action<T, object>? CreateSetter<T>(this PropertyInfo property)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      if (property.DeclaringType is null)
        throw new ArgumentException(
          $"The specified property '{nameof(property)}' has no {nameof(MemberInfo.DeclaringType)}");

      var setter = property.GetSetMethod(true);
      if (setter is null)
        throw new ArgumentException($"The specified property '{property.Name}' does not have a setter method.");

      var genericMethod = GenericSetterCreationMethod?.MakeGenericMethod(property.DeclaringType, property.PropertyType);

      if (genericMethod is null)
        throw new InvalidOperationException($"Couldn't create a generic method for '{nameof(property)}'");

      return genericMethod.Invoke(null, new object[] { setter }) as Action<T, object>;
    }

    private static Action<T, object> CreateSetterGeneric<T, TValue>(MethodInfo setter) where T : class
    {
      var setterTypedDelegate = (Action<T, TValue>) Delegate.CreateDelegate(typeof(Action<T, TValue>), setter);
      var setterDelegate = (Action<T, object>) ((instance, value) => setterTypedDelegate(instance, (TValue) value));

      return setterDelegate;
    }
  }
}
