# Quantity

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1183`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Quantity, int>`, `IComparable<Quantity>`, `IConvertible`, `IEquatable<Quantity>`, `IFormattable`, `IParsable<Quantity>`, `ISpanFormattable`, `ISpanParsable<Quantity>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<Quantity>`, `IComparisonOperators<Quantity, Quantity, bool>`, `IEqualityOperators<Quantity, Quantity, bool>`

A non-negative count of items, valid under [QuantityRules](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantityrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Quantity
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Quantity, int>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Quantity>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Quantity>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Quantity>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Quantity>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Quantity>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.Quantity, Rocket.Surgery.LaunchPad.StrongTypes.Quantity, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Quantity, Rocket.Surgery.LaunchPad.StrongTypes.Quantity, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#asprimitive) : `int`
- [CompareTo(Quantity?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#compareto-quantity) : `int`
- [Equals(Quantity?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#equals-quantity) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(int)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#from-int) : [Quantity](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Quantity?, Quantity?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#op-equality-quantity-quantity) : `bool` `static`
- [op_GreaterThan(Quantity, Quantity)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#op-greaterthan-quantity-quantity) : `bool` `static`
- [op_GreaterThanOrEqual(Quantity, Quantity)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#op-greaterthanorequal-quantity-quantity) : `bool` `static`
- [op_Inequality(Quantity?, Quantity?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#op-inequality-quantity-quantity) : `bool` `static`
- [op_LessThan(Quantity, Quantity)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#op-lessthan-quantity-quantity) : `bool` `static`
- [op_LessThanOrEqual(Quantity, Quantity)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#op-lessthanorequal-quantity-quantity) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#parse-string-iformatprovider) : [Quantity](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#parse-readonlyspan-char-iformatprovider) : [Quantity](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#parse-readonlyspan-byte-iformatprovider) : [Quantity](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(int, Quantity)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#tryfrom-int-quantity) : `bool` `static`
- [TryParse(string?, IFormatProvider?, Quantity)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#tryparse-string-iformatprovider-quantity) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, Quantity)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#tryparse-readonlyspan-char-iformatprovider-quantity) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, Quantity)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/quantity/methods.md#tryparse-readonlyspan-byte-iformatprovider-quantity) : `bool` `static`
