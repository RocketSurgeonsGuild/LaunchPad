# CurrencyCode Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives.md)
- Type: [CurrencyCode](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycode.md)
- Kind: `Methods`
- Members: `10`

An ISO 4217 currency code, valid under [CurrencyCodeRules](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycoderules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class CurrencyCode
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-currencycode"></a>

## CompareTo(CurrencyCode?)

- Name: `CompareTo(CurrencyCode?)`
- Returns: `int`

```csharp
public sealed class CurrencyCode
{
    public int CompareTo(
        CurrencyCode? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([CurrencyCode?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycode.md))

<a id="equals"></a>
<a id="equals-currencycode"></a>

## Equals(CurrencyCode?)

- Name: `Equals(CurrencyCode?)`
- Returns: `bool`

```csharp
public sealed class CurrencyCode
{
    public bool Equals(
        CurrencyCode? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([CurrencyCode?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycode.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class CurrencyCode
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
<a id="from-string"></a>

## From(string)

- Name: `From(string)`
- Modifiers: `static`
- Returns: [CurrencyCode](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycode.md)

```csharp
public sealed class CurrencyCode
{
    public static CurrencyCode From(
        string value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public sealed class CurrencyCode
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
<a id="op-equality-currencycode-currencycode"></a>

## op_Equality(CurrencyCode?, CurrencyCode?)

- Name: `op_Equality(CurrencyCode?, CurrencyCode?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class CurrencyCode
{
    public static bool operator ==(
        CurrencyCode? left,
        CurrencyCode? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([CurrencyCode?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycode.md))
- `right` ([CurrencyCode?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycode.md))

<a id="op-inequality"></a>
<a id="op-inequality-currencycode-currencycode"></a>

## op_Inequality(CurrencyCode?, CurrencyCode?)

- Name: `op_Inequality(CurrencyCode?, CurrencyCode?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class CurrencyCode
{
    public static bool operator !=(
        CurrencyCode? left,
        CurrencyCode? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([CurrencyCode?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycode.md))
- `right` ([CurrencyCode?](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycode.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class CurrencyCode
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.

<a id="tryfrom"></a>
<a id="tryfrom-string-currencycode"></a>

## TryFrom(string, CurrencyCode)

- Name: `TryFrom(string, CurrencyCode)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class CurrencyCode
{
    public static bool TryFrom(
        string value,
        out CurrencyCode result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([CurrencyCode](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/currencycode.md))
