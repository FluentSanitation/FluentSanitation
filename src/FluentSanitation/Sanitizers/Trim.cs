using FluentSanitation.Internal;

namespace FluentSanitation.Sanitizers
{
  public static partial class RuleBuilderExtensions
  {
    public static RuleBuilder<T, string?> Trim<T>(this RuleBuilder<T, string?> ruleBuilder) =>
      ruleBuilder.SetSanitizer(s => s?.Trim());
  }
}
