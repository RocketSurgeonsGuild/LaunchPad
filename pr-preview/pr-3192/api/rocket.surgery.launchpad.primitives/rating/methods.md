# Rating Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives.md)
- Type: [Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md)
- Kind: `Methods`
- Members: `23`

A rating from 1 to 5, valid under [RatingRules](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/ratingrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `int`

```csharp
public sealed class Rating
{
    public int AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-rating"></a>

## CompareTo(Rating?)

- Name: `CompareTo(Rating?)`
- Returns: `int`

```csharp
public sealed class Rating
{
    public int CompareTo(
        Rating? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Rating?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="equals"></a>
<a id="equals-rating"></a>

## Equals(Rating?)

- Name: `Equals(Rating?)`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public bool Equals(
        Rating? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Rating?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Rating
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
- Returns: [Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md)

```csharp
public sealed class Rating
{
    public static Rating From(
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
public sealed class Rating
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
<a id="op-equality-rating-rating"></a>

## op_Equality(Rating?, Rating?)

- Name: `op_Equality(Rating?, Rating?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool operator ==(
        Rating? left,
        Rating? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Rating?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))
- `right` ([Rating?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-rating-rating"></a>

## op_GreaterThan(Rating, Rating)

- Name: `op_GreaterThan(Rating, Rating)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool operator >(Rating left, Rating right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))
- `right` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-rating-rating"></a>

## op_GreaterThanOrEqual(Rating, Rating)

- Name: `op_GreaterThanOrEqual(Rating, Rating)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool operator >=(Rating left, Rating right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))
- `right` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="op-inequality"></a>
<a id="op-inequality-rating-rating"></a>

## op_Inequality(Rating?, Rating?)

- Name: `op_Inequality(Rating?, Rating?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool operator !=(
        Rating? left,
        Rating? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Rating?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))
- `right` ([Rating?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-rating-rating"></a>

## op_LessThan(Rating, Rating)

- Name: `op_LessThan(Rating, Rating)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool operator <(Rating left, Rating right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))
- `right` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-rating-rating"></a>

## op_LessThanOrEqual(Rating, Rating)

- Name: `op_LessThanOrEqual(Rating, Rating)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool operator <=(Rating left, Rating right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))
- `right` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md)

```csharp
public sealed class Rating
{
    public static Rating Parse(
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
- Returns: [Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md)

```csharp
public sealed class Rating
{
    public static Rating Parse(
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
- Returns: [Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md)

```csharp
public sealed class Rating
{
    public static Rating Parse(
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
public sealed class Rating
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
public sealed class Rating
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
public sealed class Rating
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
public sealed class Rating
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
<a id="tryfrom-int-rating"></a>

## TryFrom(int, Rating)

- Name: `TryFrom(int, Rating)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool TryFrom(
        int value,
        out Rating result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`int`)
- `result` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-rating"></a>

## TryParse(string?, IFormatProvider?, Rating)

- Name: `TryParse(string?, IFormatProvider?, Rating)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out Rating result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="tryparse-readonlyspan-char-iformatprovider-rating"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, Rating)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, Rating)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out Rating result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-rating"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, Rating)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, Rating)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Rating
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out Rating result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([Rating](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/rating.md))
