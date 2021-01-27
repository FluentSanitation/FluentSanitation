using System;
using FluentSanitation.Internal;

namespace FluentSanitation.Sanitizers
{
  public static partial class RuleBuilderExtensions
  {
    public static RuleBuilder<T, int?> NullIfEmpty<T>(this RuleBuilder<T, int?> ruleBuilder) =>
      ruleBuilder.SetSanitizer(v => (v ?? 0) == 0 ? null : v);

    public static RuleBuilder<T, long?> NullIfEmpty<T>(this RuleBuilder<T, long?> ruleBuilder) =>
      ruleBuilder.SetSanitizer(v => (v ?? 0) == 0 ? null : v);

    public static RuleBuilder<T, string?> NullIfEmpty<T>(this RuleBuilder<T, string?> ruleBuilder) =>
      ruleBuilder.SetSanitizer(s => String.IsNullOrWhiteSpace(s) ? null : s);
  }
}
