# ZeroAllocValidator Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Blazor](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.zeroalloc.blazor.md)
- Type: [ZeroAllocValidator](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.zeroalloc.blazor/zeroallocvalidator.md)
- Kind: `Methods`
- Members: `1`

Validates the model of the surrounding `Forms.EditForm` with the ZeroAlloc validator registered for it, and reports the failures through the form's `Forms.EditContext` so the validation summary and messages show them.

<a id="dispose"></a>

## Dispose

- Name: `Dispose`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.f470c0fad9a519f7357ef1c1f00f2210962f5308/src/ZeroAlloc.Blazor/ZeroAllocValidator.cs#L38)

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
