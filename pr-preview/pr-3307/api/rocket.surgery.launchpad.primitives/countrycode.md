# CountryCode

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1258`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.e1cd1b3273152fe1b84a7e7cf201a85e915d90b0/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_CountryCode.g.cs)
- Implements: `IStrongType<CountryCode, string>`, `IComparable<CountryCode>`, `IConvertible`, `IEquatable<CountryCode>`, `IParsable<CountryCode>`, `ISpanParsable<CountryCode>`, `IEqualityOperators<CountryCode, CountryCode, bool>`

An ISO 3166-1 alpha-2 country code, valid under [CountryCodeRules](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycoderules.md).

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

- [AsPrimitive](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#asprimitive) : `string`
- [CompareTo(CountryCode?)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#compareto-countrycode) : `int`
- [Equals(CountryCode?)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#equals-countrycode) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#from-string) : [CountryCode](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(CountryCode?, CountryCode?)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#op-equality-countrycode-countrycode) : `bool` `static`
- [op_Inequality(CountryCode?, CountryCode?)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#op-inequality-countrycode-countrycode) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, CountryCode)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/countrycode/methods.md#tryfrom-string-countrycode) : `bool` `static`
