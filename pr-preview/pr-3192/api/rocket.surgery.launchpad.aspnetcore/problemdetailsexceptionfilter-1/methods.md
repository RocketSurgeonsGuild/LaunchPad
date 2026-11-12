# ProblemDetailsExceptionFilter<T> Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [ProblemDetailsExceptionFilter<T>](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.aspnetcore/problemdetailsexceptionfilter-1.md)
- Kind: `Methods`
- Members: `2`

An exception filter that catches the given problem details exception and uses the given status code to return the result

<a id="onexception"></a>
<a id="onexception-exceptioncontext"></a>

## OnException(ExceptionContext)

- Name: `OnException(ExceptionContext)`

Called after an action has thrown an `Exception`.

```csharp
public abstract class ProblemDetailsExceptionFilter<T>
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
public abstract class ProblemDetailsExceptionFilter<T>
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
