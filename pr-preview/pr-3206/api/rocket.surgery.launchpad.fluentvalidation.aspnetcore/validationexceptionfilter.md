# ValidationExceptionFilter

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3206/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Validation`
- Target framework: `net10.0`
- Implements: `IAsyncExceptionFilter`, `IExceptionFilter`, `IFilterMetadata`

Captures the validation exception

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

public class ValidationExceptionFilter
    : Microsoft.AspNetCore.Mvc.Filters.IAsyncExceptionFilter,
      Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter,
      Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata
{
    // ...
}
```

## Constructors

- [ValidationExceptionFilter](/LaunchPad/pr-preview/pr-3206/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationexceptionfilter/constructors.md#constructor)

## Methods

- [OnException(ExceptionContext)](/LaunchPad/pr-preview/pr-3206/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationexceptionfilter/methods.md#onexception-exceptioncontext) -- Called after an action has thrown an `Exception`.
- [OnExceptionAsync(ExceptionContext)](/LaunchPad/pr-preview/pr-3206/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationexceptionfilter/methods.md#onexceptionasync-exceptioncontext) : `Task` -- Called after an action has thrown an `Exception`.
