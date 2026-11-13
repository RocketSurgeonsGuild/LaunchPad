# IpAddress Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives.md)
- Type: [IpAddress](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddress.md)
- Kind: `Methods`
- Members: `11`

An IPv4 or IPv6 address, valid under [IpAddressRules](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddressrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class IpAddress
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-ipaddress"></a>

## CompareTo(IpAddress?)

- Name: `CompareTo(IpAddress?)`
- Returns: `int`

```csharp
public sealed class IpAddress
{
    public int CompareTo(
        IpAddress? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([IpAddress?](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddress.md))

<a id="equals"></a>
<a id="equals-ipaddress"></a>

## Equals(IpAddress?)

- Name: `Equals(IpAddress?)`
- Returns: `bool`

```csharp
public sealed class IpAddress
{
    public bool Equals(
        IpAddress? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([IpAddress?](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddress.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class IpAddress
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
- Returns: [IpAddress](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddress.md)

```csharp
public sealed class IpAddress
{
    public static IpAddress From(
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
public sealed class IpAddress
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
<a id="op-equality-ipaddress-ipaddress"></a>

## op_Equality(IpAddress?, IpAddress?)

- Name: `op_Equality(IpAddress?, IpAddress?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class IpAddress
{
    public static bool operator ==(
        IpAddress? left,
        IpAddress? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([IpAddress?](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddress.md))
- `right` ([IpAddress?](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddress.md))

<a id="op-inequality"></a>
<a id="op-inequality-ipaddress-ipaddress"></a>

## op_Inequality(IpAddress?, IpAddress?)

- Name: `op_Inequality(IpAddress?, IpAddress?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class IpAddress
{
    public static bool operator !=(
        IpAddress? left,
        IpAddress? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([IpAddress?](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddress.md))
- `right` ([IpAddress?](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddress.md))

<a id="toipaddress"></a>

## ToIPAddress

- Name: `ToIPAddress`
- Returns: `IPAddress`

Returns the address as an `Net.IPAddress`.

```csharp
public sealed class IpAddress
{
    public IPAddress ToIPAddress()
    {
        // ...
    }
}
```

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class IpAddress
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
<a id="tryfrom-string-ipaddress"></a>

## TryFrom(string, IpAddress)

- Name: `TryFrom(string, IpAddress)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class IpAddress
{
    public static bool TryFrom(
        string value,
        out IpAddress result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([IpAddress](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ipaddress.md))
