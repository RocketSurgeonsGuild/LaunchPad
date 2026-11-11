# ValidationExceptionFilter Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [ValidationExceptionFilter](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationexceptionfilter.md)
- Kind: `Methods`
- Members: `2`

Captures the validation exception

<a id="onexception"></a>
<a id="onexception-exceptioncontext"></a>

## OnException(ExceptionContext)

- Name: `OnException(ExceptionContext)`

Called after an action has thrown an `Exception`.

```csharp
public class ValidationExceptionFilter
{
    public void OnException(
        ExceptionContext context)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ExceptionContext`)
  The `Filters.ExceptionContext`.

<a id="onexceptionasync"></a>
<a id="onexceptionasync-exceptioncontext"></a>

## OnExceptionAsync(ExceptionContext)

- Name: `OnExceptionAsync(ExceptionContext)`
- Returns: `Task`

Called after an action has thrown an `Exception`.

```csharp
public class ValidationExceptionFilter
{
    public Task OnExceptionAsync(
        ExceptionContext context)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ExceptionContext`)
  The `Filters.ExceptionContext`.

## Returns

`Task` -- A `Tasks.Task` that on completion indicates the filter has executed.
