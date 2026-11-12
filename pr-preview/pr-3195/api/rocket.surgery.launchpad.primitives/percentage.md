# Percentage

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Percentage, decimal>`, `IComparable<Percentage>`, `IConvertible`, `IEquatable<Percentage>`, `IFormattable`, `IParsable<Percentage>`, `ISpanFormattable`, `ISpanParsable<Percentage>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<Percentage>`, `IComparisonOperators<Percentage, Percentage, bool>`, `IEqualityOperators<Percentage, Percentage, bool>`

A percentage from 0 to 100, valid under [PercentageRules](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentagerules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Percentage
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Percentage, decimal>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Percentage>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Percentage>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Percentage>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Percentage>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Percentage>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.Percentage, Rocket.Surgery.LaunchPad.StrongTypes.Percentage, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Percentage, Rocket.Surgery.LaunchPad.StrongTypes.Percentage, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#asprimitive) : `decimal`
- [CompareTo(Percentage?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#compareto-percentage) : `int`
- [Equals(Percentage?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#equals-percentage) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(decimal)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#from-decimal) : [Percentage](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Percentage?, Percentage?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#op-equality-percentage-percentage) : `bool` `static`
- [op_GreaterThan(Percentage, Percentage)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#op-greaterthan-percentage-percentage) : `bool` `static`
- [op_GreaterThanOrEqual(Percentage, Percentage)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#op-greaterthanorequal-percentage-percentage) : `bool` `static`
- [op_Inequality(Percentage?, Percentage?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#op-inequality-percentage-percentage) : `bool` `static`
- [op_LessThan(Percentage, Percentage)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#op-lessthan-percentage-percentage) : `bool` `static`
- [op_LessThanOrEqual(Percentage, Percentage)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#op-lessthanorequal-percentage-percentage) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#parse-string-iformatprovider) : [Percentage](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#parse-readonlyspan-char-iformatprovider) : [Percentage](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#parse-readonlyspan-byte-iformatprovider) : [Percentage](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(decimal, Percentage)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#tryfrom-decimal-percentage) : `bool` `static`
- [TryParse(string?, IFormatProvider?, Percentage)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#tryparse-string-iformatprovider-percentage) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, Percentage)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#tryparse-readonlyspan-char-iformatprovider-percentage) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, Percentage)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/percentage/methods.md#tryparse-readonlyspan-byte-iformatprovider-percentage) : `bool` `static`
