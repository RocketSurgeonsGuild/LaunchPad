# ZeroAllocValidationErrorFilter Fields

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [ZeroAllocValidationErrorFilter](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter.md)
- Kind: `Fields`
- Members: `1`

Reports each failure of a request the ZeroAlloc validators rejected as a `VALIDATION` error, with the property name and the error code as extensions.

<a id="validationcode"></a>

## ValidationCode

- Name: `ValidationCode`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.65b042458886c034a574f04a60cfe2fa08ef3e1a/src/ZeroAlloc.HotChocolate/Validation/ZeroAllocValidationErrorFilter.cs)

The error code of a validation failure.

```csharp
public const static string ValidationCode
```
