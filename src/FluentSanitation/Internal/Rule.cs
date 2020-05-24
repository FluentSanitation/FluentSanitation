using System;

namespace FluentSanitation.Internal
{
  public class Rule<T>
  {
    public T Action { get; set; } = default!;
    public string PropertyName { get; set; } = default!;
    public string RuleSet { get; set; } = String.Empty;
  }
}
