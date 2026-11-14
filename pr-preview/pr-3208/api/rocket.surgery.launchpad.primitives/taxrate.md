# TaxRate

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1188`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<TaxRate, decimal>`, `IComparable<TaxRate>`, `IConvertible`, `IEquatable<TaxRate>`, `IFormattable`, `IParsable<TaxRate>`, `ISpanFormattable`, `ISpanParsable<TaxRate>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<TaxRate>`, `IComparisonOperators<TaxRate, TaxRate, bool>`, `IEqualityOperators<TaxRate, TaxRate, bool>`

A tax rate as a percentage, valid under [TaxRateRules](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxraterules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class TaxRate
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.TaxRate, decimal>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.TaxRate>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.TaxRate>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.TaxRate>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.TaxRate>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.TaxRate>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.TaxRate, Rocket.Surgery.LaunchPad.StrongTypes.TaxRate, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.TaxRate, Rocket.Surgery.LaunchPad.StrongTypes.TaxRate, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#asprimitive) : `decimal`
- [CompareTo(TaxRate?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#compareto-taxrate) : `int`
- [Equals(TaxRate?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#equals-taxrate) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(decimal)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#from-decimal) : [TaxRate](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(TaxRate?, TaxRate?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#op-equality-taxrate-taxrate) : `bool` `static`
- [op_GreaterThan(TaxRate, TaxRate)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#op-greaterthan-taxrate-taxrate) : `bool` `static`
- [op_GreaterThanOrEqual(TaxRate, TaxRate)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#op-greaterthanorequal-taxrate-taxrate) : `bool` `static`
- [op_Inequality(TaxRate?, TaxRate?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#op-inequality-taxrate-taxrate) : `bool` `static`
- [op_LessThan(TaxRate, TaxRate)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#op-lessthan-taxrate-taxrate) : `bool` `static`
- [op_LessThanOrEqual(TaxRate, TaxRate)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#op-lessthanorequal-taxrate-taxrate) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#parse-string-iformatprovider) : [TaxRate](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#parse-readonlyspan-char-iformatprovider) : [TaxRate](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#parse-readonlyspan-byte-iformatprovider) : [TaxRate](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(decimal, TaxRate)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#tryfrom-decimal-taxrate) : `bool` `static`
- [TryParse(string?, IFormatProvider?, TaxRate)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#tryparse-string-iformatprovider-taxrate) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, TaxRate)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#tryparse-readonlyspan-char-iformatprovider-taxrate) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, TaxRate)](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/taxrate/methods.md#tryparse-readonlyspan-byte-iformatprovider-taxrate) : `bool` `static`
