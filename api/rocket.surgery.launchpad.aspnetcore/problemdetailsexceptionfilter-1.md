# ProblemDetailsExceptionFilter<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-beta.328`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Filters`
- Target framework: `net10.0`
- Implements: `IAsyncExceptionFilter`, `IExceptionFilter`, `IFilterMetadata`

An exception filter that catches the given problem details exception and uses the given status code to return the result

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Filters;

public abstract class ProblemDetailsExceptionFilter<T>
    : Microsoft.AspNetCore.Mvc.Filters.IAsyncExceptionFilter,
      Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter,
      Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata
    where T : Exception, IProblemDetailsData
{
    // ...
}
```

## Remarks

Create the problem details filter

## Methods

- [OnException(ExceptionContext)](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/problemdetailsexceptionfilter-1/methods.md#onexception-exceptioncontext) -- Called after an action has thrown an `Exception`.
- [OnExceptionAsync(ExceptionContext)](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/problemdetailsexceptionfilter-1/methods.md#onexceptionasync-exceptioncontext) : `Task` -- Called after an action has thrown an `Exception`.
