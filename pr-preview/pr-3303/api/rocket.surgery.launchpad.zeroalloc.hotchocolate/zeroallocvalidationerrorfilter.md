# ZeroAllocValidationErrorFilter

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-pr.1254`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Validation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3303-merge.Sha.af4f47555b5ed577aca4ba95e6332df625343e98/src/ZeroAlloc.HotChocolate/Validation/ZeroAllocValidationErrorFilter.cs)
- Implements: `IErrorFilter`

Reports each failure of a request the ZeroAlloc validators rejected as a `VALIDATION` error, with the property name and the error code as extensions.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Validation;

public sealed class ZeroAllocValidationErrorFilter
    : HotChocolate.Execution.IErrorFilter
{
    // ...
}
```

## Constructors

- [ZeroAllocValidationErrorFilter](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter/constructors.md#constructor)

## Methods

- [OnError(IError)](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter/methods.md#onerror-ierror) : `IError` -- OnError is called whenever an error occurred during execution of a query.

## Fields

- [ValidationCode](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter/fields.md#validationcode) : `string` `static` -- The error code of a validation failure.
