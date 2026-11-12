# Latitude Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives.md)
- Type: [Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md)
- Kind: `Methods`
- Members: `23`

A latitude in decimal degrees, valid under [LatitudeRules](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latituderules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `double`

```csharp
public sealed class Latitude
{
    public double AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-latitude"></a>

## CompareTo(Latitude?)

- Name: `CompareTo(Latitude?)`
- Returns: `int`

```csharp
public sealed class Latitude
{
    public int CompareTo(
        Latitude? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Latitude?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="equals"></a>
<a id="equals-latitude"></a>

## Equals(Latitude?)

- Name: `Equals(Latitude?)`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public bool Equals(
        Latitude? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Latitude?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Latitude
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
- Returns: [Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md)

```csharp
public sealed class Latitude
{
    public static Latitude From(
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
public sealed class Latitude
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
<a id="op-equality-latitude-latitude"></a>

## op_Equality(Latitude?, Latitude?)

- Name: `op_Equality(Latitude?, Latitude?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool operator ==(
        Latitude? left,
        Latitude? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Latitude?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))
- `right` ([Latitude?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-latitude-latitude"></a>

## op_GreaterThan(Latitude, Latitude)

- Name: `op_GreaterThan(Latitude, Latitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool operator >(Latitude left, Latitude right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))
- `right` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-latitude-latitude"></a>

## op_GreaterThanOrEqual(Latitude, Latitude)

- Name: `op_GreaterThanOrEqual(Latitude, Latitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool operator >=(Latitude left, Latitude right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))
- `right` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="op-inequality"></a>
<a id="op-inequality-latitude-latitude"></a>

## op_Inequality(Latitude?, Latitude?)

- Name: `op_Inequality(Latitude?, Latitude?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool operator !=(
        Latitude? left,
        Latitude? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Latitude?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))
- `right` ([Latitude?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-latitude-latitude"></a>

## op_LessThan(Latitude, Latitude)

- Name: `op_LessThan(Latitude, Latitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool operator <(Latitude left, Latitude right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))
- `right` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-latitude-latitude"></a>

## op_LessThanOrEqual(Latitude, Latitude)

- Name: `op_LessThanOrEqual(Latitude, Latitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool operator <=(Latitude left, Latitude right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))
- `right` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md)

```csharp
public sealed class Latitude
{
    public static Latitude Parse(
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
- Returns: [Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md)

```csharp
public sealed class Latitude
{
    public static Latitude Parse(
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
- Returns: [Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md)

```csharp
public sealed class Latitude
{
    public static Latitude Parse(
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
public sealed class Latitude
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
public sealed class Latitude
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
public sealed class Latitude
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
public sealed class Latitude
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
<a id="tryfrom-double-latitude"></a>

## TryFrom(double, Latitude)

- Name: `TryFrom(double, Latitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool TryFrom(
        double value,
        out Latitude result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`double`)
- `result` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-latitude"></a>

## TryParse(string?, IFormatProvider?, Latitude)

- Name: `TryParse(string?, IFormatProvider?, Latitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out Latitude result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="tryparse-readonlyspan-char-iformatprovider-latitude"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, Latitude)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, Latitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out Latitude result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-latitude"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, Latitude)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, Latitude)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Latitude
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out Latitude result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([Latitude](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/latitude.md))
