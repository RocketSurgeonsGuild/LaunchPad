# Quarter Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives.md)
- Type: [Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md)
- Kind: `Methods`
- Members: `23`

A quarter of the year, valid under [QuarterRules](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarterrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `int`

```csharp
public sealed class Quarter
{
    public int AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-quarter"></a>

## CompareTo(Quarter?)

- Name: `CompareTo(Quarter?)`
- Returns: `int`

```csharp
public sealed class Quarter
{
    public int CompareTo(
        Quarter? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Quarter?](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="equals"></a>
<a id="equals-quarter"></a>

## Equals(Quarter?)

- Name: `Equals(Quarter?)`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public bool Equals(
        Quarter? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Quarter?](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Quarter
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
- Returns: [Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md)

```csharp
public sealed class Quarter
{
    public static Quarter From(
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
public sealed class Quarter
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
<a id="op-equality-quarter-quarter"></a>

## op_Equality(Quarter?, Quarter?)

- Name: `op_Equality(Quarter?, Quarter?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool operator ==(
        Quarter? left,
        Quarter? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quarter?](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))
- `right` ([Quarter?](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-quarter-quarter"></a>

## op_GreaterThan(Quarter, Quarter)

- Name: `op_GreaterThan(Quarter, Quarter)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool operator >(Quarter left, Quarter right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))
- `right` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-quarter-quarter"></a>

## op_GreaterThanOrEqual(Quarter, Quarter)

- Name: `op_GreaterThanOrEqual(Quarter, Quarter)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool operator >=(Quarter left, Quarter right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))
- `right` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="op-inequality"></a>
<a id="op-inequality-quarter-quarter"></a>

## op_Inequality(Quarter?, Quarter?)

- Name: `op_Inequality(Quarter?, Quarter?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool operator !=(
        Quarter? left,
        Quarter? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quarter?](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))
- `right` ([Quarter?](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-quarter-quarter"></a>

## op_LessThan(Quarter, Quarter)

- Name: `op_LessThan(Quarter, Quarter)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool operator <(Quarter left, Quarter right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))
- `right` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-quarter-quarter"></a>

## op_LessThanOrEqual(Quarter, Quarter)

- Name: `op_LessThanOrEqual(Quarter, Quarter)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool operator <=(Quarter left, Quarter right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))
- `right` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md)

```csharp
public sealed class Quarter
{
    public static Quarter Parse(
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
- Returns: [Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md)

```csharp
public sealed class Quarter
{
    public static Quarter Parse(
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
- Returns: [Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md)

```csharp
public sealed class Quarter
{
    public static Quarter Parse(
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
public sealed class Quarter
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
public sealed class Quarter
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
public sealed class Quarter
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
public sealed class Quarter
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
<a id="tryfrom-int-quarter"></a>

## TryFrom(int, Quarter)

- Name: `TryFrom(int, Quarter)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool TryFrom(
        int value,
        out Quarter result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`int`)
- `result` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-quarter"></a>

## TryParse(string?, IFormatProvider?, Quarter)

- Name: `TryParse(string?, IFormatProvider?, Quarter)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out Quarter result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="tryparse-readonlyspan-char-iformatprovider-quarter"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, Quarter)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, Quarter)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out Quarter result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-quarter"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, Quarter)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, Quarter)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quarter
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out Quarter result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([Quarter](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/quarter.md))
