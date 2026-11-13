# ExpirationDate Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives.md)
- Type: [ExpirationDate](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md)
- Kind: `Methods`
- Members: `18`

The last day something is valid, valid under [ExpirationDateRules](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdaterules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `DateOnly`

```csharp
public sealed class ExpirationDate
{
    public DateOnly AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-expirationdate"></a>

## CompareTo(ExpirationDate?)

- Name: `CompareTo(ExpirationDate?)`
- Returns: `int`

```csharp
public sealed class ExpirationDate
{
    public int CompareTo(
        ExpirationDate? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([ExpirationDate?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md))

<a id="equals"></a>
<a id="equals-expirationdate"></a>

## Equals(ExpirationDate?)

- Name: `Equals(ExpirationDate?)`
- Returns: `bool`

```csharp
public sealed class ExpirationDate
{
    public bool Equals(
        ExpirationDate? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([ExpirationDate?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class ExpirationDate
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
<a id="from-dateonly"></a>

## From(DateOnly)

- Name: `From(DateOnly)`
- Modifiers: `static`
- Returns: [ExpirationDate](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md)

```csharp
public sealed class ExpirationDate
{
    public static ExpirationDate From(
        DateOnly value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`DateOnly`)

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public sealed class ExpirationDate
{
    public override int GetHashCode()
    {
        // ...
    }
}
```

## Returns

`int` -- A hash code for the current object.

<a id="isexpiredon"></a>
<a id="isexpiredon-dateonly"></a>

## IsExpiredOn(DateOnly)

- Name: `IsExpiredOn(DateOnly)`
- Returns: `bool`

Whether it has expired on `date`: the expiration date is the last valid day, so it expires the day after.

```csharp
public sealed class ExpirationDate
{
    public bool IsExpiredOn(
        DateOnly date)
    {
        // ...
    }
}
```

## Parameters

- `date` (`DateOnly`)

<a id="op-equality"></a>
<a id="op-equality-expirationdate-expirationdate"></a>

## op_Equality(ExpirationDate?, ExpirationDate?)

- Name: `op_Equality(ExpirationDate?, ExpirationDate?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class ExpirationDate
{
    public static bool operator ==(
        ExpirationDate? left,
        ExpirationDate? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([ExpirationDate?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md))
- `right` ([ExpirationDate?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md))

<a id="op-inequality"></a>
<a id="op-inequality-expirationdate-expirationdate"></a>

## op_Inequality(ExpirationDate?, ExpirationDate?)

- Name: `op_Inequality(ExpirationDate?, ExpirationDate?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class ExpirationDate
{
    public static bool operator !=(
        ExpirationDate? left,
        ExpirationDate? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([ExpirationDate?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md))
- `right` ([ExpirationDate?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [ExpirationDate](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md)

```csharp
public sealed class ExpirationDate
{
    public static ExpirationDate Parse(
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
- Returns: [ExpirationDate](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md)

```csharp
public sealed class ExpirationDate
{
    public static ExpirationDate Parse(
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

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class ExpirationDate
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
public sealed class ExpirationDate
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
public sealed class ExpirationDate
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
public sealed class ExpirationDate
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
<a id="tryfrom-dateonly-expirationdate"></a>

## TryFrom(DateOnly, ExpirationDate)

- Name: `TryFrom(DateOnly, ExpirationDate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class ExpirationDate
{
    public static bool TryFrom(
        DateOnly value,
        out ExpirationDate result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`DateOnly`)
- `result` ([ExpirationDate](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-expirationdate"></a>

## TryParse(string?, IFormatProvider?, ExpirationDate)

- Name: `TryParse(string?, IFormatProvider?, ExpirationDate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class ExpirationDate
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out ExpirationDate result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([ExpirationDate](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md))

<a id="tryparse-readonlyspan-char-iformatprovider-expirationdate"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, ExpirationDate)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, ExpirationDate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class ExpirationDate
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out ExpirationDate result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([ExpirationDate](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/expirationdate.md))
