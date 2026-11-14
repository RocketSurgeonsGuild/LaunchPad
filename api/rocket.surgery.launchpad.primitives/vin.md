# Vin

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.347`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Vin, string>`, `IComparable<Vin>`, `IConvertible`, `IEquatable<Vin>`, `IParsable<Vin>`, `ISpanParsable<Vin>`, `IEqualityOperators<Vin, Vin, bool>`

A vehicle identification number, valid under [VinRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/vinrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Vin
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Vin, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Vin>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Vin>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Vin>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Vin>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Vin, Rocket.Surgery.LaunchPad.StrongTypes.Vin, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#asprimitive) : `string`
- [CompareTo(Vin?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#compareto-vin) : `int`
- [Equals(Vin?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#equals-vin) : `bool`
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#from-string) : [Vin](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin.md) `static`
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Vin?, Vin?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#op-equality-vin-vin) : `bool` `static`
- [op_Inequality(Vin?, Vin?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#op-inequality-vin-vin) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, Vin)](/LaunchPad/api/rocket.surgery.launchpad.primitives/vin/methods.md#tryfrom-string-vin) : `bool` `static`
