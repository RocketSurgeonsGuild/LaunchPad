# Price

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1218`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Price, decimal>`, `IComparable<Price>`, `IConvertible`, `IEquatable<Price>`, `IFormattable`, `IParsable<Price>`, `ISpanFormattable`, `ISpanParsable<Price>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<Price>`, `IComparisonOperators<Price, Price, bool>`, `IEqualityOperators<Price, Price, bool>`

A non-negative price, valid under [PriceRules](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/pricerules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Price
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Price, decimal>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Price>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Price>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Price>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Price>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Price>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.Price, Rocket.Surgery.LaunchPad.StrongTypes.Price, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Price, Rocket.Surgery.LaunchPad.StrongTypes.Price, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#asprimitive) : `decimal`
- [CompareTo(Price?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#compareto-price) : `int`
- [Equals(Price?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#equals-price) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(decimal)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#from-decimal) : [Price](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Price?, Price?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#op-equality-price-price) : `bool` `static`
- [op_GreaterThan(Price, Price)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#op-greaterthan-price-price) : `bool` `static`
- [op_GreaterThanOrEqual(Price, Price)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#op-greaterthanorequal-price-price) : `bool` `static`
- [op_Inequality(Price?, Price?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#op-inequality-price-price) : `bool` `static`
- [op_LessThan(Price, Price)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#op-lessthan-price-price) : `bool` `static`
- [op_LessThanOrEqual(Price, Price)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#op-lessthanorequal-price-price) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#parse-string-iformatprovider) : [Price](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#parse-readonlyspan-char-iformatprovider) : [Price](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#parse-readonlyspan-byte-iformatprovider) : [Price](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(decimal, Price)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#tryfrom-decimal-price) : `bool` `static`
- [TryParse(string?, IFormatProvider?, Price)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#tryparse-string-iformatprovider-price) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, Price)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#tryparse-readonlyspan-char-iformatprovider-price) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, Price)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/price/methods.md#tryparse-readonlyspan-byte-iformatprovider-price) : `bool` `static`
