using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

using FluentSanitation.AspNetCore.Abstractions;
using FluentSanitation.Internal;

namespace FluentSanitation.AspNetCore
{
  public class SanitationModelBinder : IModelBinder
  {
    private readonly ISanitizer _sanitizer;

    public SanitationModelBinder(ISanitizer sanitizer)
    {
      _sanitizer = sanitizer;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
      var factory = bindingContext.ActionContext.HttpContext.RequestServices
        .GetRequiredService<ISanitationModelBinderFactory>();

      var binder = factory.CreateBinder(new ModelBinderFactoryContext {Metadata = bindingContext.ModelMetadata});

      await binder.BindModelAsync(bindingContext);

      if (bindingContext.Result.Model is null)
        return;

      var model = _sanitizer.Sanitize(bindingContext.Result.Model);

      if (model is null)
        return;

      bindingContext.Model = model;
    }
  }
}
