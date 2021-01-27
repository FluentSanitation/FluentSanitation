using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

using FluentSanitation.AspNetCore.Abstractions;

namespace FluentSanitation.AspNetCore
{
  public class SanitationModelBinderFactory : ISanitationModelBinderFactory
  {
    private readonly IModelBinderFactory _factory;

    public SanitationModelBinderFactory(IModelMetadataProvider metadataProvider,
      IOptions<MvcOptions> options,
      IServiceProvider serviceProvider)
    {
      var me = options.Value.ModelBinderProviders
        .FirstOrDefault(t => t.GetType() == typeof(SanitationModelBinderProvider));

      if (me is not null)
        options.Value.ModelBinderProviders.Remove(me);

      _factory = new ModelBinderFactory(metadataProvider, options, serviceProvider);
    }

    public IModelBinder CreateBinder(ModelBinderFactoryContext context) =>
      _factory.CreateBinder(context);
  }
}
