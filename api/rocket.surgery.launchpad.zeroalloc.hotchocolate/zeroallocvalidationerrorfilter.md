# ZeroAllocValidationErrorFilter

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-beta.452`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Validation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.166abe0de80666ff26996d1f2603a1f6ba81c14c/src/ZeroAlloc.HotChocolate/Validation/ZeroAllocValidationErrorFilter.cs)
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

- [ZeroAllocValidationErrorFilter](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter/constructors.md#constructor)

## Methods

- [OnError(IError)](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter/methods.md#onerror-ierror) : `IError` -- OnError is called whenever an error occurred during execution of a query.

## Fields

- [ValidationCode](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter/fields.md#validationcode) : `string` `static` -- The error code of a validation failure.
