# Month Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives.md)
- Type: [Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md)
- Kind: `Methods`
- Members: `23`

A month of the year, valid under [MonthRules](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/monthrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `int`

```csharp
public sealed class Month
{
    public int AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-month"></a>

## CompareTo(Month?)

- Name: `CompareTo(Month?)`
- Returns: `int`

```csharp
public sealed class Month
{
    public int CompareTo(
        Month? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Month?](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="equals"></a>
<a id="equals-month"></a>

## Equals(Month?)

- Name: `Equals(Month?)`
- Returns: `bool`

```csharp
public sealed class Month
{
    public bool Equals(
        Month? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Month?](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Month
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
<a id="from-int"></a>

## From(int)

- Name: `From(int)`
- Modifiers: `static`
- Returns: [Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md)

```csharp
public sealed class Month
{
    public static Month From(
        int value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`int`)

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public sealed class Month
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
<a id="op-equality-month-month"></a>

## op_Equality(Month?, Month?)

- Name: `op_Equality(Month?, Month?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool operator ==(
        Month? left,
        Month? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Month?](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))
- `right` ([Month?](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-month-month"></a>

## op_GreaterThan(Month, Month)

- Name: `op_GreaterThan(Month, Month)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool operator >(Month left, Month right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))
- `right` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-month-month"></a>

## op_GreaterThanOrEqual(Month, Month)

- Name: `op_GreaterThanOrEqual(Month, Month)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool operator >=(Month left, Month right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))
- `right` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="op-inequality"></a>
<a id="op-inequality-month-month"></a>

## op_Inequality(Month?, Month?)

- Name: `op_Inequality(Month?, Month?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool operator !=(
        Month? left,
        Month? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Month?](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))
- `right` ([Month?](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-month-month"></a>

## op_LessThan(Month, Month)

- Name: `op_LessThan(Month, Month)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool operator <(Month left, Month right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))
- `right` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-month-month"></a>

## op_LessThanOrEqual(Month, Month)

- Name: `op_LessThanOrEqual(Month, Month)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool operator <=(Month left, Month right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))
- `right` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md)

```csharp
public sealed class Month
{
    public static Month Parse(
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
- Returns: [Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md)

```csharp
public sealed class Month
{
    public static Month Parse(
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
- Returns: [Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md)

```csharp
public sealed class Month
{
    public static Month Parse(
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
public sealed class Month
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
public sealed class Month
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
public sealed class Month
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
public sealed class Month
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
<a id="tryfrom-int-month"></a>

## TryFrom(int, Month)

- Name: `TryFrom(int, Month)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool TryFrom(
        int value,
        out Month result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`int`)
- `result` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-month"></a>

## TryParse(string?, IFormatProvider?, Month)

- Name: `TryParse(string?, IFormatProvider?, Month)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out Month result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="tryparse-readonlyspan-char-iformatprovider-month"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, Month)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, Month)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out Month result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-month"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, Month)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, Month)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Month
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out Month result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([Month](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/month.md))
