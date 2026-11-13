# LanguageCode

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<LanguageCode, string>`, `IComparable<LanguageCode>`, `IConvertible`, `IEquatable<LanguageCode>`, `IParsable<LanguageCode>`, `ISpanParsable<LanguageCode>`, `IEqualityOperators<LanguageCode, LanguageCode, bool>`

A BCP 47 language tag, valid under [LanguageCodeRules](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecoderules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class LanguageCode
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.LanguageCode, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.LanguageCode>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.LanguageCode>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.LanguageCode>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.LanguageCode>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.LanguageCode, Rocket.Surgery.LaunchPad.StrongTypes.LanguageCode, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#asprimitive) : `string`
- [CompareTo(LanguageCode?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#compareto-languagecode) : `int`
- [Equals(LanguageCode?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#equals-languagecode) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#from-string) : [LanguageCode](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(LanguageCode?, LanguageCode?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#op-equality-languagecode-languagecode) : `bool` `static`
- [op_Inequality(LanguageCode?, LanguageCode?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#op-inequality-languagecode-languagecode) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, LanguageCode)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/languagecode/methods.md#tryfrom-string-languagecode) : `bool` `static`
