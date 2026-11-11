# BusinessDate

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.340`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<BusinessDate, DateOnly>`, `IComparable<BusinessDate>`, `IEquatable<BusinessDate>`, `IFormattable`, `IParsable<BusinessDate>`, `ISpanFormattable`, `ISpanParsable<BusinessDate>`, `IUtf8SpanFormattable`, `IEqualityOperators<BusinessDate, BusinessDate, bool>`

A weekday business is done on, valid under [BusinessDateRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdaterules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class BusinessDate
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.BusinessDate, System.DateOnly>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.BusinessDate>,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.BusinessDate>,
      System.IFormattable,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.BusinessDate>,
      System.ISpanFormattable,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.BusinessDate>,
      System.IUtf8SpanFormattable,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.BusinessDate, Rocket.Surgery.LaunchPad.StrongTypes.BusinessDate, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#asprimitive) : `DateOnly`
- [CompareTo(BusinessDate?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#compareto-businessdate) : `int`
- [Equals(BusinessDate?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#equals-businessdate) : `bool`
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(DateOnly)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#from-dateonly) : [BusinessDate](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate.md) `static`
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(BusinessDate?, BusinessDate?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#op-equality-businessdate-businessdate) : `bool` `static`
- [op_Inequality(BusinessDate?, BusinessDate?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#op-inequality-businessdate-businessdate) : `bool` `static`
- [Parse(string, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#parse-string-iformatprovider) : [BusinessDate](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate.md) `static`
- [Parse(ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#parse-readonlyspan-char-iformatprovider) : [BusinessDate](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate.md) `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [ToString(string?, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#tostring-string-iformatprovider) : `string`
- [TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#tryformat-span-char-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#tryformat-span-byte-int-readonlyspan-char-iformatprovider) : `bool`
- [TryFrom(DateOnly, BusinessDate)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#tryfrom-dateonly-businessdate) : `bool` `static`
- [TryParse(string?, IFormatProvider?, BusinessDate)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#tryparse-string-iformatprovider-businessdate) : `bool` `static`
- [TryParse(ReadOnlySpan<char>, IFormatProvider?, BusinessDate)](/LaunchPad/api/rocket.surgery.launchpad.primitives/businessdate/methods.md#tryparse-readonlyspan-char-iformatprovider-businessdate) : `bool` `static`
