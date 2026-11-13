# Percentage Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives.md)
- Type: [Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md)
- Kind: `Methods`
- Members: `23`

A percentage from 0 to 100, valid under [PercentageRules](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentagerules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `decimal`

```csharp
public sealed class Percentage
{
    public decimal AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-percentage"></a>

## CompareTo(Percentage?)

- Name: `CompareTo(Percentage?)`
- Returns: `int`

```csharp
public sealed class Percentage
{
    public int CompareTo(
        Percentage? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Percentage?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="equals"></a>
<a id="equals-percentage"></a>

## Equals(Percentage?)

- Name: `Equals(Percentage?)`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public bool Equals(
        Percentage? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Percentage?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Percentage
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
- Returns: [Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md)

```csharp
public sealed class Percentage
{
    public static Percentage From(
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
public sealed class Percentage
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
<a id="op-equality-percentage-percentage"></a>

## op_Equality(Percentage?, Percentage?)

- Name: `op_Equality(Percentage?, Percentage?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool operator ==(
        Percentage? left,
        Percentage? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Percentage?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))
- `right` ([Percentage?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-percentage-percentage"></a>

## op_GreaterThan(Percentage, Percentage)

- Name: `op_GreaterThan(Percentage, Percentage)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool operator >(Percentage left, Percentage right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))
- `right` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-percentage-percentage"></a>

## op_GreaterThanOrEqual(Percentage, Percentage)

- Name: `op_GreaterThanOrEqual(Percentage, Percentage)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool operator >=(Percentage left, Percentage right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))
- `right` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="op-inequality"></a>
<a id="op-inequality-percentage-percentage"></a>

## op_Inequality(Percentage?, Percentage?)

- Name: `op_Inequality(Percentage?, Percentage?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool operator !=(
        Percentage? left,
        Percentage? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Percentage?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))
- `right` ([Percentage?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-percentage-percentage"></a>

## op_LessThan(Percentage, Percentage)

- Name: `op_LessThan(Percentage, Percentage)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool operator <(Percentage left, Percentage right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))
- `right` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-percentage-percentage"></a>

## op_LessThanOrEqual(Percentage, Percentage)

- Name: `op_LessThanOrEqual(Percentage, Percentage)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool operator <=(Percentage left, Percentage right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))
- `right` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md)

```csharp
public sealed class Percentage
{
    public static Percentage Parse(
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
- Returns: [Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md)

```csharp
public sealed class Percentage
{
    public static Percentage Parse(
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
- Returns: [Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md)

```csharp
public sealed class Percentage
{
    public static Percentage Parse(
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
public sealed class Percentage
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
public sealed class Percentage
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
public sealed class Percentage
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
public sealed class Percentage
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
<a id="tryfrom-decimal-percentage"></a>

## TryFrom(decimal, Percentage)

- Name: `TryFrom(decimal, Percentage)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool TryFrom(
        decimal value,
        out Percentage result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`decimal`)
- `result` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-percentage"></a>

## TryParse(string?, IFormatProvider?, Percentage)

- Name: `TryParse(string?, IFormatProvider?, Percentage)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out Percentage result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="tryparse-readonlyspan-char-iformatprovider-percentage"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, Percentage)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, Percentage)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out Percentage result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-percentage"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, Percentage)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, Percentage)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Percentage
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out Percentage result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([Percentage](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/percentage.md))
