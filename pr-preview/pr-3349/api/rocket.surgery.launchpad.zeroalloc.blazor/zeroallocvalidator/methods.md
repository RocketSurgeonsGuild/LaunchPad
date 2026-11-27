# ZeroAllocValidator Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Blazor](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.zeroalloc.blazor.md)
- Type: [ZeroAllocValidator](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.zeroalloc.blazor/zeroallocvalidator.md)
- Kind: `Methods`
- Members: `1`

Validates the model of the surrounding `Forms.EditForm` with the ZeroAlloc validator registered for it, and reports the failures through the form's `Forms.EditContext` so the validation summary and messages show them.

<a id="dispose"></a>

## Dispose

- Name: `Dispose`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3349-merge.Sha.70168524ff777a6c4fd1a4bf57611d64095a2c00/src/ZeroAlloc.Blazor/ZeroAllocValidator.cs#L42)

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
