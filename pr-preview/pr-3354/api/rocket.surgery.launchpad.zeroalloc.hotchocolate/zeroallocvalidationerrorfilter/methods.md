# ZeroAllocValidationErrorFilter Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [ZeroAllocValidationErrorFilter](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter.md)
- Kind: `Methods`
- Members: `1`

Reports each failure of a request the ZeroAlloc validators rejected as a `VALIDATION` error, with the property name and the error code as extensions.

<a id="onerror"></a>
<a id="onerror-ierror"></a>

## OnError(IError)

- Name: `OnError(IError)`
- Returns: `IError`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3354-merge.Sha.770f51e280ea81405e6ea681ce5c1987ac1bab2a/src/ZeroAlloc.HotChocolate/Validation/ZeroAllocValidationErrorFilter.cs#L21-L37)

OnError is called whenever an error occurred during execution of a query.

```csharp
public sealed class ZeroAllocValidationErrorFilter
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
