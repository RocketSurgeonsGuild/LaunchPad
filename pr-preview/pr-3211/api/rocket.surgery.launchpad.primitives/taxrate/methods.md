# TaxRate Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives.md)
- Type: [TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md)
- Kind: `Methods`
- Members: `23`

A tax rate as a percentage, valid under [TaxRateRules](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxraterules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `decimal`

```csharp
public sealed class TaxRate
{
    public decimal AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-taxrate"></a>

## CompareTo(TaxRate?)

- Name: `CompareTo(TaxRate?)`
- Returns: `int`

```csharp
public sealed class TaxRate
{
    public int CompareTo(
        TaxRate? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([TaxRate?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="equals"></a>
<a id="equals-taxrate"></a>

## Equals(TaxRate?)

- Name: `Equals(TaxRate?)`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public bool Equals(
        TaxRate? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([TaxRate?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class TaxRate
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
- Returns: [TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md)

```csharp
public sealed class TaxRate
{
    public static TaxRate From(
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
public sealed class TaxRate
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
<a id="op-equality-taxrate-taxrate"></a>

## op_Equality(TaxRate?, TaxRate?)

- Name: `op_Equality(TaxRate?, TaxRate?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool operator ==(
        TaxRate? left,
        TaxRate? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([TaxRate?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))
- `right` ([TaxRate?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-taxrate-taxrate"></a>

## op_GreaterThan(TaxRate, TaxRate)

- Name: `op_GreaterThan(TaxRate, TaxRate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool operator >(TaxRate left, TaxRate right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))
- `right` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-taxrate-taxrate"></a>

## op_GreaterThanOrEqual(TaxRate, TaxRate)

- Name: `op_GreaterThanOrEqual(TaxRate, TaxRate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool operator >=(TaxRate left, TaxRate right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))
- `right` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="op-inequality"></a>
<a id="op-inequality-taxrate-taxrate"></a>

## op_Inequality(TaxRate?, TaxRate?)

- Name: `op_Inequality(TaxRate?, TaxRate?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool operator !=(
        TaxRate? left,
        TaxRate? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([TaxRate?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))
- `right` ([TaxRate?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-taxrate-taxrate"></a>

## op_LessThan(TaxRate, TaxRate)

- Name: `op_LessThan(TaxRate, TaxRate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool operator <(TaxRate left, TaxRate right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))
- `right` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-taxrate-taxrate"></a>

## op_LessThanOrEqual(TaxRate, TaxRate)

- Name: `op_LessThanOrEqual(TaxRate, TaxRate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool operator <=(TaxRate left, TaxRate right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))
- `right` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md)

```csharp
public sealed class TaxRate
{
    public static TaxRate Parse(
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
- Returns: [TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md)

```csharp
public sealed class TaxRate
{
    public static TaxRate Parse(
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
- Returns: [TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md)

```csharp
public sealed class TaxRate
{
    public static TaxRate Parse(
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
public sealed class TaxRate
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
public sealed class TaxRate
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
public sealed class TaxRate
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
public sealed class TaxRate
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
<a id="tryfrom-decimal-taxrate"></a>

## TryFrom(decimal, TaxRate)

- Name: `TryFrom(decimal, TaxRate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool TryFrom(
        decimal value,
        out TaxRate result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`decimal`)
- `result` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-taxrate"></a>

## TryParse(string?, IFormatProvider?, TaxRate)

- Name: `TryParse(string?, IFormatProvider?, TaxRate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out TaxRate result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="tryparse-readonlyspan-char-iformatprovider-taxrate"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, TaxRate)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, TaxRate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out TaxRate result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-taxrate"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, TaxRate)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, TaxRate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class TaxRate
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out TaxRate result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([TaxRate](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.primitives/taxrate.md))
