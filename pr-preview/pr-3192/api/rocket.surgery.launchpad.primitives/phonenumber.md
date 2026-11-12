# PhoneNumber

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1180`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<PhoneNumber, string>`, `IComparable<PhoneNumber>`, `IConvertible`, `IEquatable<PhoneNumber>`, `IParsable<PhoneNumber>`, `ISpanParsable<PhoneNumber>`, `IEqualityOperators<PhoneNumber, PhoneNumber, bool>`

A phone number in E.164 form, valid under [PhoneNumberRules](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumberrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class PhoneNumber
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.PhoneNumber, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.PhoneNumber>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.PhoneNumber>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.PhoneNumber>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.PhoneNumber>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.PhoneNumber, Rocket.Surgery.LaunchPad.StrongTypes.PhoneNumber, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#asprimitive) : `string`
- [CompareTo(PhoneNumber?)](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#compareto-phonenumber) : `int`
- [Equals(PhoneNumber?)](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#equals-phonenumber) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#from-string) : [PhoneNumber](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(PhoneNumber?, PhoneNumber?)](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#op-equality-phonenumber-phonenumber) : `bool` `static`
- [op_Inequality(PhoneNumber?, PhoneNumber?)](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#op-inequality-phonenumber-phonenumber) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, PhoneNumber)](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/phonenumber/methods.md#tryfrom-string-phonenumber) : `bool` `static`
