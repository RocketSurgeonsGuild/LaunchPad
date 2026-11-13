# LanguageCodeRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1183`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongTypeTemplate<string>`

The rules of a language code: a BCP 47 language tag that .NET knows as a culture, such as `en`, `en-US` or `zh-Hans-CN`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class LanguageCodeRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- The tag must name a predefined culture ( `CultureInfo.GetCultureInfo`), so well-formed tags .NET has no data for ( `tlh`, `de-CH-1996`), private use tags ( `x-private`) and the invariant culture (the empty string) are rejected.
- BCP 47 tags are case-insensitive, with a conventional casing (RFC 5646 §2.1.1). SuperStrong validates without changing the value, so the tag must be written as .NET names the culture ( `en-US`, not `EN-us` or `en_US`).
- What .NET knows comes from the platform's ICU data, so the accepted tags can differ between platforms; in invariant globalization mode ( `InvariantGlobalization`) every tag is rejected.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/languagecoderules/methods.md#define) : `StrongTypeDefinition<string>` `static`
