# Rating

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1183`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Rating, int>`, `IComparable<Rating>`, `IConvertible`, `IEquatable<Rating>`, `IFormattable`, `IParsable<Rating>`, `ISpanFormattable`, `ISpanParsable<Rating>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<Rating>`, `IComparisonOperators<Rating, Rating, bool>`, `IEqualityOperators<Rating, Rating, bool>`

A rating from 1 to 5, valid under [RatingRules](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/ratingrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Rating
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Rating, int>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Rating>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Rating>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Rating>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Rating>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Rating>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.Rating, Rocket.Surgery.LaunchPad.StrongTypes.Rating, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Rating, Rocket.Surgery.LaunchPad.StrongTypes.Rating, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#asprimitive) : `int`
- [CompareTo(Rating?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#compareto-rating) : `int`
- [Equals(Rating?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#equals-rating) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(int)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#from-int) : [Rating](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Rating?, Rating?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#op-equality-rating-rating) : `bool` `static`
- [op_GreaterThan(Rating, Rating)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#op-greaterthan-rating-rating) : `bool` `static`
- [op_GreaterThanOrEqual(Rating, Rating)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#op-greaterthanorequal-rating-rating) : `bool` `static`
- [op_Inequality(Rating?, Rating?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#op-inequality-rating-rating) : `bool` `static`
- [op_LessThan(Rating, Rating)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#op-lessthan-rating-rating) : `bool` `static`
- [op_LessThanOrEqual(Rating, Rating)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#op-lessthanorequal-rating-rating) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#parse-string-iformatprovider) : [Rating](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#parse-readonlyspan-char-iformatprovider) : [Rating](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#parse-readonlyspan-byte-iformatprovider) : [Rating](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(int, Rating)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#tryfrom-int-rating) : `bool` `static`
- [TryParse(string?, IFormatProvider?, Rating)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#tryparse-string-iformatprovider-rating) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, Rating)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#tryparse-readonlyspan-char-iformatprovider-rating) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, Rating)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/rating/methods.md#tryparse-readonlyspan-byte-iformatprovider-rating) : `bool` `static`
