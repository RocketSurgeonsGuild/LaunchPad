# BusinessDate Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives.md)
- Type: [BusinessDate](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md)
- Kind: `Methods`
- Members: `17`

A weekday business is done on, valid under [BusinessDateRules](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdaterules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `DateOnly`

```csharp
public sealed class BusinessDate
{
    public DateOnly AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-businessdate"></a>

## CompareTo(BusinessDate?)

- Name: `CompareTo(BusinessDate?)`
- Returns: `int`

```csharp
public sealed class BusinessDate
{
    public int CompareTo(
        BusinessDate? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([BusinessDate?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md))

<a id="equals"></a>
<a id="equals-businessdate"></a>

## Equals(BusinessDate?)

- Name: `Equals(BusinessDate?)`
- Returns: `bool`

```csharp
public sealed class BusinessDate
{
    public bool Equals(
        BusinessDate? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([BusinessDate?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class BusinessDate
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
- Returns: [BusinessDate](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md)

```csharp
public sealed class BusinessDate
{
    public static BusinessDate From(
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
public sealed class BusinessDate
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
<a id="op-equality-businessdate-businessdate"></a>

## op_Equality(BusinessDate?, BusinessDate?)

- Name: `op_Equality(BusinessDate?, BusinessDate?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class BusinessDate
{
    public static bool operator ==(
        BusinessDate? left,
        BusinessDate? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([BusinessDate?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md))
- `right` ([BusinessDate?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md))

<a id="op-inequality"></a>
<a id="op-inequality-businessdate-businessdate"></a>

## op_Inequality(BusinessDate?, BusinessDate?)

- Name: `op_Inequality(BusinessDate?, BusinessDate?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class BusinessDate
{
    public static bool operator !=(
        BusinessDate? left,
        BusinessDate? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([BusinessDate?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md))
- `right` ([BusinessDate?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [BusinessDate](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md)

```csharp
public sealed class BusinessDate
{
    public static BusinessDate Parse(
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
- Returns: [BusinessDate](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md)

```csharp
public sealed class BusinessDate
{
    public static BusinessDate Parse(
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
public sealed class BusinessDate
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
public sealed class BusinessDate
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
public sealed class BusinessDate
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
public sealed class BusinessDate
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
<a id="tryfrom-dateonly-businessdate"></a>

## TryFrom(DateOnly, BusinessDate)

- Name: `TryFrom(DateOnly, BusinessDate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class BusinessDate
{
    public static bool TryFrom(
        DateOnly value,
        out BusinessDate result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`DateOnly`)
- `result` ([BusinessDate](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-businessdate"></a>

## TryParse(string?, IFormatProvider?, BusinessDate)

- Name: `TryParse(string?, IFormatProvider?, BusinessDate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class BusinessDate
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out BusinessDate result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([BusinessDate](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md))

<a id="tryparse-readonlyspan-char-iformatprovider-businessdate"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, BusinessDate)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, BusinessDate)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class BusinessDate
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out BusinessDate result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([BusinessDate](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/businessdate.md))
