# ZeroAllocValidationErrorFilter Fields

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [ZeroAllocValidationErrorFilter](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter.md)
- Kind: `Fields`
- Members: `1`

Reports each failure of a request the ZeroAlloc validators rejected as a `VALIDATION` error, with the property name and the error code as extensions.

<a id="validationcode"></a>

## ValidationCode

- Name: `ValidationCode`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.b3468f8f7f271d2dc63ee1d3333c2bcdd48e93a2/src/ZeroAlloc.HotChocolate/Validation/ZeroAllocValidationErrorFilter.cs)

The error code of a validation failure.

```csharp
public const static string ValidationCode
```
