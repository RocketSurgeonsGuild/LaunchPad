# MacAddress Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives.md)
- Type: [MacAddress](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddress.md)
- Kind: `Methods`
- Members: `10`

An EUI-48 MAC address, valid under [MacAddressRules](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddressrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class MacAddress
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-macaddress"></a>

## CompareTo(MacAddress?)

- Name: `CompareTo(MacAddress?)`
- Returns: `int`

```csharp
public sealed class MacAddress
{
    public int CompareTo(
        MacAddress? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([MacAddress?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddress.md))

<a id="equals"></a>
<a id="equals-macaddress"></a>

## Equals(MacAddress?)

- Name: `Equals(MacAddress?)`
- Returns: `bool`

```csharp
public sealed class MacAddress
{
    public bool Equals(
        MacAddress? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([MacAddress?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddress.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class MacAddress
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
- Returns: [MacAddress](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddress.md)

```csharp
public sealed class MacAddress
{
    public static MacAddress From(
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
public sealed class MacAddress
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
<a id="op-equality-macaddress-macaddress"></a>

## op_Equality(MacAddress?, MacAddress?)

- Name: `op_Equality(MacAddress?, MacAddress?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class MacAddress
{
    public static bool operator ==(
        MacAddress? left,
        MacAddress? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([MacAddress?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddress.md))
- `right` ([MacAddress?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddress.md))

<a id="op-inequality"></a>
<a id="op-inequality-macaddress-macaddress"></a>

## op_Inequality(MacAddress?, MacAddress?)

- Name: `op_Inequality(MacAddress?, MacAddress?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class MacAddress
{
    public static bool operator !=(
        MacAddress? left,
        MacAddress? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([MacAddress?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddress.md))
- `right` ([MacAddress?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddress.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class MacAddress
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
<a id="tryfrom-string-macaddress"></a>

## TryFrom(string, MacAddress)

- Name: `TryFrom(string, MacAddress)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class MacAddress
{
    public static bool TryFrom(
        string value,
        out MacAddress result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([MacAddress](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddress.md))
