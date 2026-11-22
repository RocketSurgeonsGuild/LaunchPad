# AuthorizationErrorFilter Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [AuthorizationErrorFilter](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authorizationerrorfilter.md)
- Kind: `Methods`
- Members: `1`

Reports a field HotChocolate's authorization rejected as the `Unauthorized` or `Forbidden` case of a union response is reported: `UNAUTHENTICATED` or `FORBIDDEN` as the code, and the case name in `extensions.case`. A client reads one shape, whether the edge or the mediator pipeline rejected the call.

<a id="onerror"></a>
<a id="onerror-ierror"></a>

## OnError(IError)

- Name: `OnError(IError)`
- Returns: `IError`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.HotChocolate/Authorization/AuthorizationErrorFilter.cs#L23-L29)

OnError is called whenever an error occurred during execution of a query.

```csharp
public sealed class AuthorizationErrorFilter
{
    public IError OnError(
        IError error)
    {
        // ...
    }
}
```

## Parameters

- `error` (`IError`)
  The error that occurred. This argument cannot be null.

## Returns

`IError` -- Returns the error passed in to this filter or a rewritten error. It is not allowed to return null.
