# Price Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives.md)
- Type: [Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md)
- Kind: `Methods`
- Members: `23`

A non-negative price, valid under [PriceRules](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/pricerules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `decimal`

```csharp
public sealed class Price
{
    public decimal AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-price"></a>

## CompareTo(Price?)

- Name: `CompareTo(Price?)`
- Returns: `int`

```csharp
public sealed class Price
{
    public int CompareTo(
        Price? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Price?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="equals"></a>
<a id="equals-price"></a>

## Equals(Price?)

- Name: `Equals(Price?)`
- Returns: `bool`

```csharp
public sealed class Price
{
    public bool Equals(
        Price? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Price?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Price
{
    public sealed override bool Equals(
        object? obj)
    {
        // ...
    }
}
```

## Parameters

- `obj` (`object?`)
  The object to compare with the current object.

## Returns

`bool` -- `true` if the specified object is equal to the current object; otherwise, `false`.

<a id="from"></a>
<a id="from-decimal"></a>

## From(decimal)

- Name: `From(decimal)`
- Modifiers: `static`
- Returns: [Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md)

```csharp
public sealed class Price
{
    public static Price From(
        decimal value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`decimal`)

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public sealed class Price
{
    public override int GetHashCode()
    {
        // ...
    }
}
```

## Returns

`int` -- A hash code for the current object.

<a id="op-equality"></a>
<a id="op-equality-price-price"></a>

## op_Equality(Price?, Price?)

- Name: `op_Equality(Price?, Price?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool operator ==(
        Price? left,
        Price? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Price?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))
- `right` ([Price?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-price-price"></a>

## op_GreaterThan(Price, Price)

- Name: `op_GreaterThan(Price, Price)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool operator >(Price left, Price right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))
- `right` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-price-price"></a>

## op_GreaterThanOrEqual(Price, Price)

- Name: `op_GreaterThanOrEqual(Price, Price)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool operator >=(Price left, Price right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))
- `right` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="op-inequality"></a>
<a id="op-inequality-price-price"></a>

## op_Inequality(Price?, Price?)

- Name: `op_Inequality(Price?, Price?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool operator !=(
        Price? left,
        Price? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Price?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))
- `right` ([Price?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-price-price"></a>

## op_LessThan(Price, Price)

- Name: `op_LessThan(Price, Price)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool operator <(Price left, Price right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))
- `right` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-price-price"></a>

## op_LessThanOrEqual(Price, Price)

- Name: `op_LessThanOrEqual(Price, Price)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool operator <=(Price left, Price right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))
- `right` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md)

```csharp
public sealed class Price
{
    public static Price Parse(
        string s,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string`)
- `provider` (`IFormatProvider?`)

<a id="parse-readonlyspan-char-iformatprovider"></a>

## Parse(ReadOnlySpan<char>, IFormatProvider?)

- Name: `Parse(ReadOnlySpan<char>, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md)

```csharp
public sealed class Price
{
    public static Price Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)

<a id="parse-readonlyspan-byte-iformatprovider"></a>

## Parse(ReadOnlySpan<byte>, IFormatProvider?)

- Name: `Parse(ReadOnlySpan<byte>, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md)

```csharp
public sealed class Price
{
    public static Price Parse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class Price
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.

<a id="tostring-string-iformatprovider"></a>

## ToString(string?, IFormatProvider?)

- Name: `ToString(string?, IFormatProvider?)`
- Returns: `string`

```csharp
public sealed class Price
{
    public string ToString(
        string? format,
        IFormatProvider? formatProvider)
    {
        // ...
    }
}
```

## Parameters

- `format` (`string?`)
- `formatProvider` (`IFormatProvider?`)

<a id="tryformat"></a>
<a id="tryformat-span-char-int-readonlyspan-char-iformatprovider"></a>

## TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)

- Name: `TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)`
- Returns: `bool`

```csharp
public sealed class Price
{
    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `destination` (`Span<char>`)
- `charsWritten` (`int`)
- `format` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)

<a id="tryformat-span-byte-int-readonlyspan-char-iformatprovider"></a>

## TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)

- Name: `TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)`
- Returns: `bool`

```csharp
public sealed class Price
{
    public bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `utf8Destination` (`Span<byte>`)
- `bytesWritten` (`int`)
- `format` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)

<a id="tryfrom"></a>
<a id="tryfrom-decimal-price"></a>

## TryFrom(decimal, Price)

- Name: `TryFrom(decimal, Price)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool TryFrom(
        decimal value,
        out Price result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`decimal`)
- `result` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-price"></a>

## TryParse(string?, IFormatProvider?, Price)

- Name: `TryParse(string?, IFormatProvider?, Price)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out Price result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="tryparse-readonlyspan-char-iformatprovider-price"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, Price)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, Price)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out Price result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-price"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, Price)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, Price)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Price
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out Price result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([Price](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/price.md))
