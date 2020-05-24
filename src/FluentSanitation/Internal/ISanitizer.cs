using System;

namespace FluentSanitation.Internal
{
  public interface ISanitizer
  {
    object? Sanitize(object instance);
  }
}
