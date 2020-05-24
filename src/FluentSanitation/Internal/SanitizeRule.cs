using System;

namespace FluentSanitation.Internal
{
  public class SanitizeRule<T> : Rule<Func<Sanitizer<T>, T, object>> where T : class
  {
  }
}
