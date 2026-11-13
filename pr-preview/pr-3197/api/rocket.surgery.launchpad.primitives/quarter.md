# Quarter

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1181`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Quarter, int>`, `IComparable<Quarter>`, `IConvertible`, `IEquatable<Quarter>`, `IFormattable`, `IParsable<Quarter>`, `ISpanFormattable`, `ISpanParsable<Quarter>`, `IUtf8SpanFormattable`, `IUtf8SpanParsable<Quarter>`, `IComparisonOperators<Quarter, Quarter, bool>`, `IEqualityOperators<Quarter, Quarter, bool>`

A quarter of the year, valid under [QuarterRules](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarterrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Quarter
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Quarter, int>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Quarter>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Quarter>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Quarter>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Quarter>,
      System.IUtf8SpanFormattable,
      System.IUtf8SpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Quarter>,
      System.Numerics.IComparisonOperators<Rocket.Surgery.LaunchPad.StrongTypes.Quarter, Rocket.Surgery.LaunchPad.StrongTypes.Quarter, bool>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Quarter, Rocket.Surgery.LaunchPad.StrongTypes.Quarter, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#asprimitive) : `int`
- [CompareTo(Quarter?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#compareto-quarter) : `int`
- [Equals(Quarter?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#equals-quarter) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(int)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#from-int) : [Quarter](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Quarter?, Quarter?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#op-equality-quarter-quarter) : `bool` `static`
- [op_GreaterThan(Quarter, Quarter)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#op-greaterthan-quarter-quarter) : `bool` `static`
- [op_GreaterThanOrEqual(Quarter, Quarter)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#op-greaterthanorequal-quarter-quarter) : `bool` `static`
- [op_Inequality(Quarter?, Quarter?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#op-inequality-quarter-quarter) : `bool` `static`
- [op_LessThan(Quarter, Quarter)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#op-lessthan-quarter-quarter) : `bool` `static`
- [op_LessThanOrEqual(Quarter, Quarter)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#op-lessthanorequal-quarter-quarter) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#parse-string-iformatprovider) : [Quarter](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#parse-readonlyspan-char-iformatprovider) : [Quarter](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter.md) `static`
- [Parse(ReadOnlySpan<byte>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#parse-readonlyspan-byte-iformatprovider) : [Quarter](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter.md) `static`
- [ToString](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(int, Quarter)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#tryfrom-int-quarter) : `bool` `static`
- [TryParse(string?, IFormatProvider?, Quarter)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#tryparse-string-iformatprovider-quarter) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, Quarter)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#tryparse-readonlyspan-char-iformatprovider-quarter) : `bool` `static`
- [TryParse(ReadOnlySpan<byte>, IFormatProvider?, Quarter)](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/quarter/methods.md#tryparse-readonlyspan-byte-iformatprovider-quarter) : `bool` `static`
