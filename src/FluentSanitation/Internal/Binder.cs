using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentSanitation.Extensions;

namespace FluentSanitation.Internal
{
  public interface IBinder
  {
    Dictionary<string, MemberInfo> GetMembers(Type t);
  }

  public class Binder : IBinder
  {
    private readonly BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    public Binder()
    {
    }

    public Binder(BindingFlags bindingFlags) => _bindingFlags = bindingFlags;

    public Dictionary<string, MemberInfo> GetMembers(Type t) =>
      t.GetAllMembers(_bindingFlags)
        .Where(m =>
          !m.GetCustomAttributes<CompilerGeneratedAttribute>(true).Any() && m switch
          {
            FieldInfo fi => !fi.IsPrivate,
            PropertyInfo pi => pi.CanWrite,
            _ => false
          })
        .GroupBy(mi => mi.Name)
        .ToDictionary(g => g.Key, g => g.First());
  }
}
