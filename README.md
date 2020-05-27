A small library for .NET to declaratively define object sanitation rules using a fluent interface and lambda expressions. Inspired by [FluentValidation](https://fluentvalidation.net/).

```
public class CustomerSanitizer : Sanitizer<Customer>
{
  public CustomerSanitizer()
  {
    RuleFor(c => c.GivenName)
      .Trim();

    RuleFor(c => c.EmailAddress)
      .NullIfEmpty()
      .Trim();

    RuleFor(c => c.Gender)
      .ToEnum<Gender>()
      .WithDefault(Gender.Unset);

    RuleFor(c => c.DateOfBirth)
      .NullIfOlderThan(130);

    RuleFor(c => c.PhoneNumber)
      .SetSanitizer(PhoneNumberFormatter);
  }

  string PhoneNumberFormatter(string? input)
  {
    // return formatted input
  }
}
```

## Get Started

Install with .NET Core CLI or NuGet.
```
dotnet add package FluentSanitation
```

For integration with ASP.NET Core, install `FluentSanitation.AspNetCore`
```
dotnet add package FluentSanitation.AspNetCore
```

## Usage (standalone)

Create a sanitizer by inheriting from `Sanitizer<T>`, declaring rules for properties as necessary.
```
public class CustomerSanitizer : Sanitizer<Customer>
{
  public CustomerSanitizer()
  {
    RuleFor(c => c.GivenName)
      .Trim();

    RuleFor(c => c.EmailAddress)
      .NullIfEmpty()
      .Trim();

    RuleFor(c => c.Gender)
      .ToEnum<Gender>()
      .WithDefault(Gender.Unset);

    RuleFor(c => c.DateOfBirth)
      .NullIfOlderThan(130);
  }
}
```

Call `Sanitize`, passing the object to sanitize.
```
var customerSanitizer = new CustomerSanitizer();

customerSanitizer.Sanitize(customer);
```

## Usage with ASP.NET Core

Add FluentSanitation to your MVC pipeline as in the following example. Sanitizers will be executed **after** model-binding and **before** model validation.
```
using FluentSanitation.AspNetCore.Extensions;

public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddControllers()
      .AddFluentSanitation();
  }
}
```

You can opt-in to automatically registering classes that inherit from `ISanitizer` by supplying a list of types to the `AddFluentSanitation()` method. The assemblies containing the listed types will be scanned for sanitizers to register with the `Microsoft.Extensions.DependencyInjection` implementation, using a `Transient` lifetime.
```
using FluentSanitation.AspNetCore.Extensions;

public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddControllers()
      .AddFluentSanitation(typeof(Startup));
  }
}
```

Sanitizers can also be registered with the `Microsoft.Extensions.DependencyInjection` implementation manually.
```
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddSingleton<ISanitizer, CustomerSanitizer>();
    services.AddTransient<ISanitizer, OrderSanitizer>();
  }
}
```

## Defining rules

Rules provide a way to declare one or more actions to perform for a given property. Rules can be defined "inline" or by calling `SetSanitizer`.

### SetSanitizer rules
```
RuleFor(o => o.SomeProperty)
  .SetSanitizer(...);
```

`SetSanitizer` operates either on the property directly or the "parent object".
```
RuleFor(o => o.SomeStringProperty)
  .SetSanitizer(s => s.Trim());
```

Operating on the "parent object" provides capability to perform advanced/conditional action.
```
RuleFor(o => o.SomeStringProperty)
  .SetSanitier(o => o.SomeStringProperty = o.SomeOtherProperty);
```

`SetSanitizer` operations can be chained, if desired.
```
RuleFor(o => o.SomeStringProperty)
  .SetSanitizer(s => s.Trim())
  .SetSanitizer(s => s.EndsWith(".") ? s : $"{s}.");
```

`SetSanitizer` can also operate on fields.
```
RuleFor(o => o.someNumberField)
  .SetSanitizer(i => Math.Max(0, i));
```

### Inline rules
`RuleFor` can be used to define a rule "inline".
```
RuleFor(o => o.SomeStringProperty, s => s.Trim());
```

The "inline" syntax supports the same overloads for property or "parent object" access as available on `SetSanitizer`.

Rules defined "inline" also return a `RuleBuilder` and can be chained with calls to `SetSanitizer`.

### Writing extension methods

Providing extension methods for `RuleBuilder<T, TProperty>` can help declare the intent of a sanitizer by abstracting away the implementation and enables re-use of common sanitizers. Consider the following example where we want to set the `DateOfBirth` property to `null` if the value indicates the person is older than 130.

Written directly into the call to `SetSanitizer` it might look like this:
```
RuleFor(o => o.DateOfBirth)
  .SetSanitizer(dob =>
  {
    if (dob is null)
      return null;

    if (dob.Value.AddYears(130) < DateTime.Now)
      return null;

    return dob.Value;
  })
```

By abstracting the logic into an extension method, such as the following:
```
public static class RuleBuilderExtensions
{
  public static RuleBuilder<T, DateTime?> NullIfOlderThan<T>(this RuleBuilder<T, DateTime?> ruleBuilder, int years) =>
    ruleBuilder.SetSanitizer(/* as above */);
}
```

Our rule declaration, now becomes:
```
RuleFor(o => o.DateOfBirth).NullIfOlderThan(130);
```

### Type rules

Sometimes you might want to perform the same action for all properties of a specific type.
```
RuleFor(typeof(string), s => s.Trim());
```

Note: Rules that act on a `Type` are run **before** any other rules.
