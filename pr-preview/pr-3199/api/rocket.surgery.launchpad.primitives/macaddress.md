# MacAddress

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1188`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<MacAddress, string>`, `IComparable<MacAddress>`, `IConvertible`, `IEquatable<MacAddress>`, `IParsable<MacAddress>`, `ISpanParsable<MacAddress>`, `IEqualityOperators<MacAddress, MacAddress, bool>`

An EUI-48 MAC address, valid under [MacAddressRules](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddressrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class MacAddress
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.MacAddress, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.MacAddress>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.MacAddress>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.MacAddress>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.MacAddress>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.MacAddress, Rocket.Surgery.LaunchPad.StrongTypes.MacAddress, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#asprimitive) : `string`
- [CompareTo(MacAddress?)](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#compareto-macaddress) : `int`
- [Equals(MacAddress?)](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#equals-macaddress) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#from-string) : [MacAddress](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(MacAddress?, MacAddress?)](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#op-equality-macaddress-macaddress) : `bool` `static`
- [op_Inequality(MacAddress?, MacAddress?)](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#op-inequality-macaddress-macaddress) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, MacAddress)](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/macaddress/methods.md#tryfrom-string-macaddress) : `bool` `static`
