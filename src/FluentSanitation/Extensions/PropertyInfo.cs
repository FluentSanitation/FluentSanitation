using System;
using System.Reflection;

namespace FluentSanitation.Extensions
{
  internal static class PropertyInfoExtensions
  {
    private static readonly MethodInfo GenericSetterCreationMethod =
      typeof(PropertyInfoExtensions).GetMethod(nameof(CreateSetterGeneric),
        BindingFlags.Static | BindingFlags.NonPublic);

    public static Action<T, object> CreateSetter<T>(this PropertyInfo property)
    {
      if (property == null) throw new ArgumentNullException(nameof(property));

      var setter = property.GetSetMethod(true);
      if (setter == null)
        throw new ArgumentException($"The specified property '{property.Name}' does not have a setter method.");

      var genericHelper = GenericSetterCreationMethod.MakeGenericMethod(property.DeclaringType, property.PropertyType);

      return (Action<T, object>)genericHelper.Invoke(null, new object[] { setter });
    }

    private static Action<T, object> CreateSetterGeneric<T, TValue>(MethodInfo setter) where T : class
    {
      var setterTypedDelegate = (Action<T, TValue>) Delegate.CreateDelegate(typeof(Action<T, TValue>), setter);
      var setterDelegate = (Action<T, object>) ((instance, value) => setterTypedDelegate(instance, (TValue) value));

      return setterDelegate;
    }
  }
}
