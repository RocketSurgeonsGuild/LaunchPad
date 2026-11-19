# ZeroAllocValidator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Blazor](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.zeroalloc.blazor.md)
- Version: `2.0.1-pr.1253`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Blazor`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/ZeroAlloc.Blazor/ZeroAllocValidator.cs)
- Inherits: `ComponentBase`
- Implements: `IDisposable`

Validates the model of the surrounding `Forms.EditForm` with the ZeroAlloc validator registered for it, and reports the failures through the form's `Forms.EditContext` so the validation summary and messages show them.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Blazor;

public sealed class ZeroAllocValidator
    : Microsoft.AspNetCore.Components.ComponentBase,
      System.IDisposable
{
    // ...
}
```

## Remarks

The validator is resolved as `Validation.ValidatorFor`1` of the model's runtime type, which needs that closed generic type at run time. ZeroAlloc.Validation registers a validator for each `[Validate]` type, so a trimmed application keeps the types it validates.

## Constructors

- [ZeroAllocValidator](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.zeroalloc.blazor/zeroallocvalidator/constructors.md#constructor)

## Methods

- [Dispose](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.zeroalloc.blazor/zeroallocvalidator/methods.md#dispose) -- Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
