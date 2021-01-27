using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentSanitation.Extensions
{
  public static class TypeExtensions
  {
    public static IEnumerable<Type> FindSanitizersInAssembliesFromType(params Type[] types) =>
      types.Select(t => t.Assembly)
        .GroupBy(a => a.FullName)
        .Select(g => g.First())
        .SelectMany(a => a.GetExportedTypes())
        .Where(t => !t.IsAbstract
          && !t.IsInterface
          && t.BaseType?.IsGenericType == true
          && t.BaseType?.GetGenericTypeDefinition() == typeof(Sanitizer<>));

    internal static IEnumerable<MemberInfo> GetAllMembers(this Type type, BindingFlags bindingFlags) =>
      type.IsInterface
        ? Enumerable.Union(type.GetInterfaces(), new[] {type})
          .SelectMany(i => i.GetMembers(bindingFlags)).Distinct()
        : type.GetMembers(bindingFlags);
  }
}
