# ExpirationDate

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.352`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<ExpirationDate, DateOnly>`, `IComparable<ExpirationDate>`, `IEquatable<ExpirationDate>`, `IFormattable`, `IParsable<ExpirationDate>`, `ISpanFormattable`, `ISpanParsable<ExpirationDate>`, `IUtf8SpanFormattable`, `IEqualityOperators<ExpirationDate, ExpirationDate, bool>`

The last day something is valid, valid under [ExpirationDateRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdaterules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class ExpirationDate
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.ExpirationDate, System.DateOnly>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.ExpirationDate>,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.ExpirationDate>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.ExpirationDate>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.ExpirationDate>,
      System.IUtf8SpanFormattable,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.ExpirationDate, Rocket.Surgery.LaunchPad.StrongTypes.ExpirationDate, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#asprimitive) : `DateOnly`
- [CompareTo(ExpirationDate?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#compareto-expirationdate) : `int`
- [Equals(ExpirationDate?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#equals-expirationdate) : `bool`
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(DateOnly)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#from-dateonly) : [ExpirationDate](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate.md) `static`
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [IsExpiredOn(DateOnly)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#isexpiredon-dateonly) : `bool` -- Whether it has expired on `date`: the expiration date is the last valid day, so it expires the day after.
- [op_Equality(ExpirationDate?, ExpirationDate?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#op-equality-expirationdate-expirationdate) : `bool` `static`
- [op_Inequality(ExpirationDate?, ExpirationDate?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#op-inequality-expirationdate-expirationdate) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#parse-string-iformatprovider) : [ExpirationDate](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#parse-readonlyspan-char-iformatprovider) : [ExpirationDate](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate.md) `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(DateOnly, ExpirationDate)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#tryfrom-dateonly-expirationdate) : `bool` `static`
- [TryParse(string?, IFormatProvider?, ExpirationDate)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#tryparse-string-iformatprovider-expirationdate) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, ExpirationDate)](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#tryparse-readonlyspan-char-iformatprovider-expirationdate) : `bool` `static`
