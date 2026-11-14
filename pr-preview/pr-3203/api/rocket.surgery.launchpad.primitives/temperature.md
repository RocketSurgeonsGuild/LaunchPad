# Temperature

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1186`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Temperature, double>`, `IComparable<Temperature>`, `IConvertible`, `IEquatable<Temperature>`, `IFormattable`, `IParsable<Temperature>`, `ISpanFormattable`, `ISpanParsable<Temperature>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<Temperature>`, `IComparisonOperators<Temperature, Temperature, bool>`, `IEqualityOperators<Temperature, Temperature, bool>`

A temperature in degrees Celsius, valid under [TemperatureRules](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperaturerules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Temperature
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Temperature, double>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Temperature>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Temperature>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Temperature>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Temperature>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Temperature>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.Temperature, Rocket.Surgery.LaunchPad.StrongTypes.Temperature, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Temperature, Rocket.Surgery.LaunchPad.StrongTypes.Temperature, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#asprimitive) : `double`
- [CompareTo(Temperature?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#compareto-temperature) : `int`
- [Equals(Temperature?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#equals-temperature) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(double)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#from-double) : [Temperature](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Temperature?, Temperature?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#op-equality-temperature-temperature) : `bool` `static`
- [op_GreaterThan(Temperature, Temperature)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#op-greaterthan-temperature-temperature) : `bool` `static`
- [op_GreaterThanOrEqual(Temperature, Temperature)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#op-greaterthanorequal-temperature-temperature) : `bool` `static`
- [op_Inequality(Temperature?, Temperature?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#op-inequality-temperature-temperature) : `bool` `static`
- [op_LessThan(Temperature, Temperature)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#op-lessthan-temperature-temperature) : `bool` `static`
- [op_LessThanOrEqual(Temperature, Temperature)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#op-lessthanorequal-temperature-temperature) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#parse-string-iformatprovider) : [Temperature](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#parse-readonlyspan-char-iformatprovider) : [Temperature](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#parse-readonlyspan-byte-iformatprovider) : [Temperature](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(double, Temperature)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#tryfrom-double-temperature) : `bool` `static`
- [TryParse(string?, IFormatProvider?, Temperature)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#tryparse-string-iformatprovider-temperature) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, Temperature)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#tryparse-readonlyspan-char-iformatprovider-temperature) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, Temperature)](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/temperature/methods.md#tryparse-readonlyspan-byte-iformatprovider-temperature) : `bool` `static`
