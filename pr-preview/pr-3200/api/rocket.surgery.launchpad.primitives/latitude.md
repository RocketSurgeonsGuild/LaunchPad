# Latitude

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Latitude, double>`, `IComparable<Latitude>`, `IConvertible`, `IEquatable<Latitude>`, `IFormattable`, `IParsable<Latitude>`, `ISpanFormattable`, `ISpanParsable<Latitude>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<Latitude>`, `IComparisonOperators<Latitude, Latitude, bool>`, `IEqualityOperators<Latitude, Latitude, bool>`

A latitude in decimal degrees, valid under [LatitudeRules](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latituderules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Latitude
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Latitude, double>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Latitude>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Latitude>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Latitude>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Latitude>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Latitude>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.Latitude, Rocket.Surgery.LaunchPad.StrongTypes.Latitude, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Latitude, Rocket.Surgery.LaunchPad.StrongTypes.Latitude, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#asprimitive) : `double`
- [CompareTo(Latitude?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#compareto-latitude) : `int`
- [Equals(Latitude?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#equals-latitude) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(double)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#from-double) : [Latitude](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Latitude?, Latitude?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#op-equality-latitude-latitude) : `bool` `static`
- [op_GreaterThan(Latitude, Latitude)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#op-greaterthan-latitude-latitude) : `bool` `static`
- [op_GreaterThanOrEqual(Latitude, Latitude)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#op-greaterthanorequal-latitude-latitude) : `bool` `static`
- [op_Inequality(Latitude?, Latitude?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#op-inequality-latitude-latitude) : `bool` `static`
- [op_LessThan(Latitude, Latitude)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#op-lessthan-latitude-latitude) : `bool` `static`
- [op_LessThanOrEqual(Latitude, Latitude)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#op-lessthanorequal-latitude-latitude) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#parse-string-iformatprovider) : [Latitude](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#parse-readonlyspan-char-iformatprovider) : [Latitude](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#parse-readonlyspan-byte-iformatprovider) : [Latitude](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(double, Latitude)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#tryfrom-double-latitude) : `bool` `static`
- [TryParse(string?, IFormatProvider?, Latitude)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#tryparse-string-iformatprovider-latitude) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, Latitude)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#tryparse-readonlyspan-char-iformatprovider-latitude) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, Latitude)](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/latitude/methods.md#tryparse-readonlyspan-byte-iformatprovider-latitude) : `bool` `static`
