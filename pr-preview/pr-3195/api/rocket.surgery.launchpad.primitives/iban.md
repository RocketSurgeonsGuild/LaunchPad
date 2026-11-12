# Iban

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Iban, string>`, `IComparable<Iban>`, `IConvertible`, `IEquatable<Iban>`, `IParsable<Iban>`, `ISpanParsable<Iban>`, `IEqualityOperators<Iban, Iban, bool>`

An IBAN in electronic format, valid under [IbanRules](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/ibanrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Iban
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Iban, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Iban>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Iban>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Iban>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Iban>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Iban, Rocket.Surgery.LaunchPad.StrongTypes.Iban, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#asprimitive) : `string`
- [CompareTo(Iban?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#compareto-iban) : `int`
- [Equals(Iban?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#equals-iban) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#from-string) : [Iban](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Iban?, Iban?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#op-equality-iban-iban) : `bool` `static`
- [op_Inequality(Iban?, Iban?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#op-inequality-iban-iban) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, Iban)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/iban/methods.md#tryfrom-string-iban) : `bool` `static`
