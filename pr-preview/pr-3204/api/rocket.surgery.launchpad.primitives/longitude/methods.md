# Longitude Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives.md)
- Type: [Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md)
- Kind: `Methods`
- Members: `23`

A longitude in decimal degrees, valid under [LongitudeRules](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longituderules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `double`

```csharp
public sealed class Longitude
{
    public double AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-longitude"></a>

## CompareTo(Longitude?)

- Name: `CompareTo(Longitude?)`
- Returns: `int`

```csharp
public sealed class Longitude
{
    public int CompareTo(
        Longitude? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Longitude?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="equals"></a>
<a id="equals-longitude"></a>

## Equals(Longitude?)

- Name: `Equals(Longitude?)`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public bool Equals(
        Longitude? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Longitude?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Longitude
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
<a id="from-double"></a>

## From(double)

- Name: `From(double)`
- Modifiers: `static`
- Returns: [Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md)

```csharp
public sealed class Longitude
{
    public static Longitude From(
        double value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`double`)

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public sealed class Longitude
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
<a id="op-equality-longitude-longitude"></a>

## op_Equality(Longitude?, Longitude?)

- Name: `op_Equality(Longitude?, Longitude?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool operator ==(
        Longitude? left,
        Longitude? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Longitude?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))
- `right` ([Longitude?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-longitude-longitude"></a>

## op_GreaterThan(Longitude, Longitude)

- Name: `op_GreaterThan(Longitude, Longitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool operator >(Longitude left, Longitude right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))
- `right` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-longitude-longitude"></a>

## op_GreaterThanOrEqual(Longitude, Longitude)

- Name: `op_GreaterThanOrEqual(Longitude, Longitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool operator >=(Longitude left, Longitude right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))
- `right` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="op-inequality"></a>
<a id="op-inequality-longitude-longitude"></a>

## op_Inequality(Longitude?, Longitude?)

- Name: `op_Inequality(Longitude?, Longitude?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool operator !=(
        Longitude? left,
        Longitude? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Longitude?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))
- `right` ([Longitude?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-longitude-longitude"></a>

## op_LessThan(Longitude, Longitude)

- Name: `op_LessThan(Longitude, Longitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool operator <(Longitude left, Longitude right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))
- `right` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-longitude-longitude"></a>

## op_LessThanOrEqual(Longitude, Longitude)

- Name: `op_LessThanOrEqual(Longitude, Longitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool operator <=(Longitude left, Longitude right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))
- `right` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md)

```csharp
public sealed class Longitude
{
    public static Longitude Parse(
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
- Returns: [Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md)

```csharp
public sealed class Longitude
{
    public static Longitude Parse(
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
- Returns: [Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md)

```csharp
public sealed class Longitude
{
    public static Longitude Parse(
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
public sealed class Longitude
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
public sealed class Longitude
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
public sealed class Longitude
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
public sealed class Longitude
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
<a id="tryfrom-double-longitude"></a>

## TryFrom(double, Longitude)

- Name: `TryFrom(double, Longitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool TryFrom(
        double value,
        out Longitude result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`double`)
- `result` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-longitude"></a>

## TryParse(string?, IFormatProvider?, Longitude)

- Name: `TryParse(string?, IFormatProvider?, Longitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out Longitude result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="tryparse-readonlyspan-char-iformatprovider-longitude"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, Longitude)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, Longitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out Longitude result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-longitude"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, Longitude)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, Longitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Longitude
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out Longitude result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([Longitude](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/longitude.md))
