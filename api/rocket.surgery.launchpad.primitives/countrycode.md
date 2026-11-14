# CountryCode

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.347`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<CountryCode, string>`, `IComparable<CountryCode>`, `IConvertible`, `IEquatable<CountryCode>`, `IParsable<CountryCode>`, `ISpanParsable<CountryCode>`, `IEqualityOperators<CountryCode, CountryCode, bool>`

An ISO 3166-1 alpha-2 country code, valid under [CountryCodeRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycoderules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class CountryCode
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.CountryCode, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.CountryCode>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.CountryCode>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.CountryCode>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.CountryCode>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.CountryCode, Rocket.Surgery.LaunchPad.StrongTypes.CountryCode, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#asprimitive) : `string`
- [CompareTo(CountryCode?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#compareto-countrycode) : `int`
- [Equals(CountryCode?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#equals-countrycode) : `bool`
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#from-string) : [CountryCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode.md) `static`
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(CountryCode?, CountryCode?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#op-equality-countrycode-countrycode) : `bool` `static`
- [op_Inequality(CountryCode?, CountryCode?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#op-inequality-countrycode-countrycode) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, CountryCode)](/LaunchPad/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#tryfrom-string-countrycode) : `bool` `static`
