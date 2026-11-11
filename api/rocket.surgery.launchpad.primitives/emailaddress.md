# EmailAddress

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.340`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<EmailAddress, string>`, `IComparable<EmailAddress>`, `IConvertible`, `IEquatable<EmailAddress>`, `IParsable<EmailAddress>`, `ISpanParsable<EmailAddress>`, `IEqualityOperators<EmailAddress, EmailAddress, bool>`

An email address, valid under [EmailAddressRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddressrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class EmailAddress
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.EmailAddress, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.EmailAddress>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.EmailAddress>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.EmailAddress>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.EmailAddress>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.EmailAddress, Rocket.Surgery.LaunchPad.StrongTypes.EmailAddress, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#asprimitive) : `string`
- [CompareTo(EmailAddress?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#compareto-emailaddress) : `int`
- [Equals(EmailAddress?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#equals-emailaddress) : `bool`
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#from-string) : [EmailAddress](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress.md) `static`
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(EmailAddress?, EmailAddress?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#op-equality-emailaddress-emailaddress) : `bool` `static`
- [op_Inequality(EmailAddress?, EmailAddress?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#op-inequality-emailaddress-emailaddress) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, EmailAddress)](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddress/methods.md#tryfrom-string-emailaddress) : `bool` `static`
