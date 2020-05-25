using System;
using FluentSanitation.AspNetCore.Abstractions;
using Microsoft.Extensions.DependencyInjection;

using FluentSanitation.Extensions;
using FluentSanitation.Internal;

namespace FluentSanitation.AspNetCore.Extensions
{
  public static class MvcBuilderExtensions
  {
    /// <summary>
    /// Add Fluent Sanitation services to the specified <c>IMvcBuilder</c>.
    /// </summary>
    /// <param name="mvcBuilder">An <c>IMvcBuilder</c></param>
    /// <param name="types">A list of types whose assemblies will be scanned for <c>Sanitizer</c>s to register</param>
    /// <returns>An <c>IMvcBuilder</c> that can be used to further configure MVC services.</returns>
    public static IMvcBuilder AddFluentSanitation(this IMvcBuilder mvcBuilder, params Type[] types)
    {
      mvcBuilder.Services.AddSingleton<ISanitationModelBinderFactory, SanitationModelBinderFactory>();

      foreach (var type in TypeExtensions.FindSanitizersInAssembliesFromType(types))
        mvcBuilder.Services.AddTransient(typeof(ISanitizer), type);

      mvcBuilder.AddMvcOptions(options =>
        options.ModelBinderProviders.Insert(0, new SanitationModelBinderProvider()));

      return mvcBuilder;
    }
  }
}
