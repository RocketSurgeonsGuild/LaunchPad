# ZeroAllocValidationErrorFilter Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [ZeroAllocValidationErrorFilter](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter.md)
- Kind: `Methods`
- Members: `1`

Reports each failure of a request the ZeroAlloc validators rejected as a `VALIDATION` error, with the property name and the error code as extensions.

<a id="onerror"></a>
<a id="onerror-ierror"></a>

## OnError(IError)

- Name: `OnError(IError)`
- Returns: `IError`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.10d9c1412b7fe8190f26ae8e00f1c59fad95faf5/src/ZeroAlloc.HotChocolate/Validation/ZeroAllocValidationErrorFilter.cs#L21-L37)

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
