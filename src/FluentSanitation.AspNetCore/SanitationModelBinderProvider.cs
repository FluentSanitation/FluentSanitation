using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

using FluentSanitation.Internal;

namespace FluentSanitation.AspNetCore
{
  public class SanitationModelBinderProvider : IModelBinderProvider
  {
    private readonly ConcurrentDictionary<Type, IModelBinder> _cache
      = new ConcurrentDictionary<Type, IModelBinder>();

    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
      var modelType = context.Metadata.ModelType;

      if (_cache.ContainsKey(modelType))
        return _cache[modelType];

      var type = typeof(ISanitizer<>).MakeGenericType(modelType);

      var sanitizer = context.Services.GetServices<ISanitizer>()
        .FirstOrDefault(s => type.IsInstanceOfType(s));

      var binder = sanitizer != null
        ? new SanitationModelBinder(sanitizer)
        : null;

      _cache[modelType] = binder!;

      return _cache[modelType];
    }
  }
}
