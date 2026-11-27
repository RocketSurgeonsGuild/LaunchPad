# ZeroAllocValidationErrorFilter Fields

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [ZeroAllocValidationErrorFilter](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter.md)
- Kind: `Fields`
- Members: `1`

Reports each failure of a request the ZeroAlloc validators rejected as a `VALIDATION` error, with the property name and the error code as extensions.

<a id="validationcode"></a>

## ValidationCode

- Name: `ValidationCode`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3351-merge.Sha.a0ae38aa18a1b9917789ec429da9c9c80b65406f/src/ZeroAlloc.HotChocolate/Validation/ZeroAllocValidationErrorFilter.cs)

The error code of a validation failure.

```csharp
public const static string ValidationCode
```
