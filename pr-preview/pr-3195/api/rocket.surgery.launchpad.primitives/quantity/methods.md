# Quantity Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives.md)
- Type: [Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md)
- Kind: `Methods`
- Members: `23`

A non-negative count of items, valid under [QuantityRules](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantityrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `int`

```csharp
public sealed class Quantity
{
    public int AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-quantity"></a>

## CompareTo(Quantity?)

- Name: `CompareTo(Quantity?)`
- Returns: `int`

```csharp
public sealed class Quantity
{
    public int CompareTo(
        Quantity? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Quantity?](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="equals"></a>
<a id="equals-quantity"></a>

## Equals(Quantity?)

- Name: `Equals(Quantity?)`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public bool Equals(
        Quantity? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Quantity?](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Quantity
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
- Returns: [Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md)

```csharp
public sealed class Quantity
{
    public static Quantity From(
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
public sealed class Quantity
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
<a id="op-equality-quantity-quantity"></a>

## op_Equality(Quantity?, Quantity?)

- Name: `op_Equality(Quantity?, Quantity?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool operator ==(
        Quantity? left,
        Quantity? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quantity?](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))
- `right` ([Quantity?](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-quantity-quantity"></a>

## op_GreaterThan(Quantity, Quantity)

- Name: `op_GreaterThan(Quantity, Quantity)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool operator >(Quantity left, Quantity right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))
- `right` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-quantity-quantity"></a>

## op_GreaterThanOrEqual(Quantity, Quantity)

- Name: `op_GreaterThanOrEqual(Quantity, Quantity)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool operator >=(Quantity left, Quantity right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))
- `right` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="op-inequality"></a>
<a id="op-inequality-quantity-quantity"></a>

## op_Inequality(Quantity?, Quantity?)

- Name: `op_Inequality(Quantity?, Quantity?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool operator !=(
        Quantity? left,
        Quantity? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quantity?](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))
- `right` ([Quantity?](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-quantity-quantity"></a>

## op_LessThan(Quantity, Quantity)

- Name: `op_LessThan(Quantity, Quantity)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool operator <(Quantity left, Quantity right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))
- `right` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-quantity-quantity"></a>

## op_LessThanOrEqual(Quantity, Quantity)

- Name: `op_LessThanOrEqual(Quantity, Quantity)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool operator <=(Quantity left, Quantity right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))
- `right` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md)

```csharp
public sealed class Quantity
{
    public static Quantity Parse(
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
- Returns: [Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md)

```csharp
public sealed class Quantity
{
    public static Quantity Parse(
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
- Returns: [Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md)

```csharp
public sealed class Quantity
{
    public static Quantity Parse(
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
public sealed class Quantity
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
public sealed class Quantity
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
public sealed class Quantity
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
public sealed class Quantity
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
<a id="tryfrom-int-quantity"></a>

## TryFrom(int, Quantity)

- Name: `TryFrom(int, Quantity)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool TryFrom(
        int value,
        out Quantity result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`int`)
- `result` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-quantity"></a>

## TryParse(string?, IFormatProvider?, Quantity)

- Name: `TryParse(string?, IFormatProvider?, Quantity)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out Quantity result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="tryparse-readonlyspan-char-iformatprovider-quantity"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, Quantity)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, Quantity)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out Quantity result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-quantity"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, Quantity)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, Quantity)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Quantity
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out Quantity result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([Quantity](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/quantity.md))
