# Month

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1184`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Month, int>`, `IComparable<Month>`, `IConvertible`, `IEquatable<Month>`, `IFormattable`, `IParsable<Month>`, `ISpanFormattable`, `ISpanParsable<Month>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<Month>`, `IComparisonOperators<Month, Month, bool>`, `IEqualityOperators<Month, Month, bool>`

A month of the year, valid under [MonthRules](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/monthrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Month
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Month, int>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Month>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Month>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Month>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Month>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Month>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.Month, Rocket.Surgery.LaunchPad.StrongTypes.Month, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Month, Rocket.Surgery.LaunchPad.StrongTypes.Month, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#asprimitive) : `int`
- [CompareTo(Month?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#compareto-month) : `int`
- [Equals(Month?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#equals-month) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(int)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#from-int) : [Month](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Month?, Month?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#op-equality-month-month) : `bool` `static`
- [op_GreaterThan(Month, Month)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#op-greaterthan-month-month) : `bool` `static`
- [op_GreaterThanOrEqual(Month, Month)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#op-greaterthanorequal-month-month) : `bool` `static`
- [op_Inequality(Month?, Month?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#op-inequality-month-month) : `bool` `static`
- [op_LessThan(Month, Month)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#op-lessthan-month-month) : `bool` `static`
- [op_LessThanOrEqual(Month, Month)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#op-lessthanorequal-month-month) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#parse-string-iformatprovider) : [Month](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#parse-readonlyspan-char-iformatprovider) : [Month](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#parse-readonlyspan-byte-iformatprovider) : [Month](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(int, Month)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#tryfrom-int-month) : `bool` `static`
- [TryParse(string?, IFormatProvider?, Month)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#tryparse-string-iformatprovider-month) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, Month)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#tryparse-readonlyspan-char-iformatprovider-month) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, Month)](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/month/methods.md#tryparse-readonlyspan-byte-iformatprovider-month) : `bool` `static`
