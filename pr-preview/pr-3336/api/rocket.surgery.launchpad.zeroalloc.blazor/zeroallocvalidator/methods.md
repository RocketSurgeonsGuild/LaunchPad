# ZeroAllocValidator Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Blazor](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.zeroalloc.blazor.md)
- Type: [ZeroAllocValidator](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.zeroalloc.blazor/zeroallocvalidator.md)
- Kind: `Methods`
- Members: `1`

Validates the model of the surrounding `Forms.EditForm` with the ZeroAlloc validator registered for it, and reports the failures through the form's `Forms.EditContext` so the validation summary and messages show them.

<a id="dispose"></a>

## Dispose

- Name: `Dispose`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/ZeroAlloc.Blazor/ZeroAllocValidator.cs#L42)

Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

```csharp
public sealed class ZeroAllocValidator
{
    public void Dispose()
    {
        // ...
    }
}
```
