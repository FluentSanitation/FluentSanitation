using System;
using FluentSanitation.AspNetCore.Abstractions;
using Microsoft.Extensions.DependencyInjection;

using FluentSanitation.Extensions;
using FluentSanitation.Internal;

namespace FluentSanitation.AspNetCore.Extensions
{
  public static class MvcBuilderExtensions
  {
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
