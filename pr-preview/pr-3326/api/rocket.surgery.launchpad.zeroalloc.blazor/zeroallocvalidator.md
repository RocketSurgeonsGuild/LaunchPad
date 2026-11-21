# ZeroAllocValidator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Blazor](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.zeroalloc.blazor.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Blazor`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3326-merge.Sha.03f97a4ece74153c5b06eb64e3df4c70a1e53a62/src/ZeroAlloc.Blazor/ZeroAllocValidator.cs)
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

The validator is resolved as `Validation.ValidatorFor`1` of the model's runtime type, which needs that closed generic type at run time. ZeroAlloc.Validation registers a validator for each `[Validate]` type, so a trimmed application keeps the types it validates. A model without a registered validator is an error. A view model `[Mutable]` fills in has one: Foundation's `LaunchPadMutableValidatorsJoint` registers the `Validator` the generator nests in it, which validates the value the view model makes, and the failures go to its properties of the same name, so the form binds to the view model and the request's rules apply.

## Constructors

- [ZeroAllocValidator](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.zeroalloc.blazor/zeroallocvalidator/constructors.md#constructor)

## Methods

- [Dispose](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.zeroalloc.blazor/zeroallocvalidator/methods.md#dispose) -- Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
