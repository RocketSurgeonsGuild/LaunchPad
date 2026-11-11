# Vin

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1168`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Vin, string>`, `IComparable<Vin>`, `IConvertible`, `IEquatable<Vin>`, `IParsable<Vin>`, `ISpanParsable<Vin>`, `IEqualityOperators<Vin, Vin, bool>`

A vehicle identification number, valid under [VinRules](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vinrules.md).

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

- [AsPrimitive](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#asprimitive) : `string`
- [CompareTo(Vin?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#compareto-vin) : `int`
- [Equals(Vin?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#equals-vin) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#from-string) : [Vin](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Vin?, Vin?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#op-equality-vin-vin) : `bool` `static`
- [op_Inequality(Vin?, Vin?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#op-inequality-vin-vin) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, Vin)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/vin/methods.md#tryfrom-string-vin) : `bool` `static`
