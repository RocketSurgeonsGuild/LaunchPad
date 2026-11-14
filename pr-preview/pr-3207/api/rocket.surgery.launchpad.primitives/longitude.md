# Longitude

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1206`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Longitude, double>`, `IComparable<Longitude>`, `IConvertible`, `IEquatable<Longitude>`, `IFormattable`, `IParsable<Longitude>`, `ISpanFormattable`, `ISpanParsable<Longitude>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<Longitude>`, `IComparisonOperators<Longitude, Longitude, bool>`, `IEqualityOperators<Longitude, Longitude, bool>`

A longitude in decimal degrees, valid under [LongitudeRules](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longituderules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Longitude
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Longitude, double>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Longitude>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Longitude>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Longitude>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Longitude>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Longitude>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.Longitude, Rocket.Surgery.LaunchPad.StrongTypes.Longitude, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Longitude, Rocket.Surgery.LaunchPad.StrongTypes.Longitude, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#asprimitive) : `double`
- [CompareTo(Longitude?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#compareto-longitude) : `int`
- [Equals(Longitude?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#equals-longitude) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(double)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#from-double) : [Longitude](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Longitude?, Longitude?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#op-equality-longitude-longitude) : `bool` `static`
- [op_GreaterThan(Longitude, Longitude)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#op-greaterthan-longitude-longitude) : `bool` `static`
- [op_GreaterThanOrEqual(Longitude, Longitude)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#op-greaterthanorequal-longitude-longitude) : `bool` `static`
- [op_Inequality(Longitude?, Longitude?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#op-inequality-longitude-longitude) : `bool` `static`
- [op_LessThan(Longitude, Longitude)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#op-lessthan-longitude-longitude) : `bool` `static`
- [op_LessThanOrEqual(Longitude, Longitude)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#op-lessthanorequal-longitude-longitude) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#parse-string-iformatprovider) : [Longitude](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#parse-readonlyspan-char-iformatprovider) : [Longitude](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#parse-readonlyspan-byte-iformatprovider) : [Longitude](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(double, Longitude)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#tryfrom-double-longitude) : `bool` `static`
- [TryParse(string?, IFormatProvider?, Longitude)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#tryparse-string-iformatprovider-longitude) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, Longitude)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#tryparse-readonlyspan-char-iformatprovider-longitude) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, Longitude)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/longitude/methods.md#tryparse-readonlyspan-byte-iformatprovider-longitude) : `bool` `static`
