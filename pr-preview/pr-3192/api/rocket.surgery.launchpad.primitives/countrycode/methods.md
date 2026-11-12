# CountryCode Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives.md)
- Type: [CountryCode](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycode.md)
- Kind: `Methods`
- Members: `10`

An ISO 3166-1 alpha-2 country code, valid under [CountryCodeRules](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycoderules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class CountryCode
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-countrycode"></a>

## CompareTo(CountryCode?)

- Name: `CompareTo(CountryCode?)`
- Returns: `int`

```csharp
public sealed class CountryCode
{
    public int CompareTo(
        CountryCode? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([CountryCode?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycode.md))

<a id="equals"></a>
<a id="equals-countrycode"></a>

## Equals(CountryCode?)

- Name: `Equals(CountryCode?)`
- Returns: `bool`

```csharp
public sealed class CountryCode
{
    public bool Equals(
        CountryCode? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([CountryCode?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycode.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class CountryCode
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
- Returns: [CountryCode](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycode.md)

```csharp
public sealed class CountryCode
{
    public static CountryCode From(
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
public sealed class CountryCode
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
<a id="op-equality-countrycode-countrycode"></a>

## op_Equality(CountryCode?, CountryCode?)

- Name: `op_Equality(CountryCode?, CountryCode?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class CountryCode
{
    public static bool operator ==(
        CountryCode? left,
        CountryCode? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([CountryCode?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycode.md))
- `right` ([CountryCode?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycode.md))

<a id="op-inequality"></a>
<a id="op-inequality-countrycode-countrycode"></a>

## op_Inequality(CountryCode?, CountryCode?)

- Name: `op_Inequality(CountryCode?, CountryCode?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class CountryCode
{
    public static bool operator !=(
        CountryCode? left,
        CountryCode? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([CountryCode?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycode.md))
- `right` ([CountryCode?](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycode.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class CountryCode
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
<a id="tryfrom-string-countrycode"></a>

## TryFrom(string, CountryCode)

- Name: `TryFrom(string, CountryCode)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class CountryCode
{
    public static bool TryFrom(
        string value,
        out CountryCode result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([CountryCode](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/countrycode.md))
