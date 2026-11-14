# CurrencyCode

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1216`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<CurrencyCode, string>`, `IComparable<CurrencyCode>`, `IConvertible`, `IEquatable<CurrencyCode>`, `IParsable<CurrencyCode>`, `ISpanParsable<CurrencyCode>`, `IEqualityOperators<CurrencyCode, CurrencyCode, bool>`

An ISO 4217 currency code, valid under [CurrencyCodeRules](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycoderules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class CurrencyCode
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.CurrencyCode, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.CurrencyCode>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.CurrencyCode>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.CurrencyCode>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.CurrencyCode>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.CurrencyCode, Rocket.Surgery.LaunchPad.StrongTypes.CurrencyCode, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#asprimitive) : `string`
- [CompareTo(CurrencyCode?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#compareto-currencycode) : `int`
- [Equals(CurrencyCode?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#equals-currencycode) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#from-string) : [CurrencyCode](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(CurrencyCode?, CurrencyCode?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#op-equality-currencycode-currencycode) : `bool` `static`
- [op_Inequality(CurrencyCode?, CurrencyCode?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#op-inequality-currencycode-currencycode) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, CurrencyCode)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/currencycode/methods.md#tryfrom-string-currencycode) : `bool` `static`
