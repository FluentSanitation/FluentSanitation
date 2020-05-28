using System;

namespace FluentSanitation.Internal
{
  public interface ISanitizer
  {
    object? Sanitize(object instance);
  }

  public interface ISanitizer<T> : ISanitizer
  {
    T Sanitize(T instance);
  }
}
