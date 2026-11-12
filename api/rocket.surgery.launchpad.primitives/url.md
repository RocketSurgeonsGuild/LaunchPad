# Url

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.345`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Url, string>`, `IComparable<Url>`, `IConvertible`, `IEquatable<Url>`, `IParsable<Url>`, `ISpanParsable<Url>`, `IEqualityOperators<Url, Url, bool>`

An absolute http or https URL, valid under [UrlRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/urlrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Url
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Url, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Url>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Url>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Url>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Url>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Url, Rocket.Surgery.LaunchPad.StrongTypes.Url, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#asprimitive) : `string`
- [CompareTo(Url?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#compareto-url) : `int`
- [Equals(Url?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#equals-url) : `bool`
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#from-string) : [Url](/LaunchPad/api/rocket.surgery.launchpad.primitives/url.md) `static`
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Url?, Url?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#op-equality-url-url) : `bool` `static`
- [op_Inequality(Url?, Url?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#op-inequality-url-url) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, Url)](/LaunchPad/api/rocket.surgery.launchpad.primitives/url/methods.md#tryfrom-string-url) : `bool` `static`
