# IpAddress

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.340`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<IpAddress, string>`, `IComparable<IpAddress>`, `IConvertible`, `IEquatable<IpAddress>`, `IParsable<IpAddress>`, `ISpanParsable<IpAddress>`, `IEqualityOperators<IpAddress, IpAddress, bool>`

An IPv4 or IPv6 address, valid under [IpAddressRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddressrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class IpAddress
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.IpAddress, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.IpAddress>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.IpAddress>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.IpAddress>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.IpAddress>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.IpAddress, Rocket.Surgery.LaunchPad.StrongTypes.IpAddress, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#asprimitive) : `string`
- [CompareTo(IpAddress?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#compareto-ipaddress) : `int`
- [Equals(IpAddress?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#equals-ipaddress) : `bool`
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#from-string) : [IpAddress](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress.md) `static`
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(IpAddress?, IpAddress?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#op-equality-ipaddress-ipaddress) : `bool` `static`
- [op_Inequality(IpAddress?, IpAddress?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#op-inequality-ipaddress-ipaddress) : `bool` `static`
- [ToIPAddress](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#toipaddress) : `IPAddress` -- Returns the address as an `Net.IPAddress`.
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, IpAddress)](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddress/methods.md#tryfrom-string-ipaddress) : `bool` `static`
