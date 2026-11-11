# Vin Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives.md)
- Type: [Vin](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vin.md)
- Kind: `Methods`
- Members: `10`

A vehicle identification number, valid under [VinRules](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vinrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class Vin
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-vin"></a>

## CompareTo(Vin?)

- Name: `CompareTo(Vin?)`
- Returns: `int`

```csharp
public sealed class Vin
{
    public int CompareTo(
        Vin? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Vin?](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vin.md))

<a id="equals"></a>
<a id="equals-vin"></a>

## Equals(Vin?)

- Name: `Equals(Vin?)`
- Returns: `bool`

```csharp
public sealed class Vin
{
    public bool Equals(
        Vin? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Vin?](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vin.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Vin
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
- Returns: [Vin](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vin.md)

```csharp
public sealed class Vin
{
    public static Vin From(
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
public sealed class Vin
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
<a id="op-equality-vin-vin"></a>

## op_Equality(Vin?, Vin?)

- Name: `op_Equality(Vin?, Vin?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Vin
{
    public static bool operator ==(
        Vin? left,
        Vin? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Vin?](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vin.md))
- `right` ([Vin?](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vin.md))

<a id="op-inequality"></a>
<a id="op-inequality-vin-vin"></a>

## op_Inequality(Vin?, Vin?)

- Name: `op_Inequality(Vin?, Vin?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Vin
{
    public static bool operator !=(
        Vin? left,
        Vin? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Vin?](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vin.md))
- `right` ([Vin?](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vin.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class Vin
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
<a id="tryfrom-string-vin"></a>

## TryFrom(string, Vin)

- Name: `TryFrom(string, Vin)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Vin
{
    public static bool TryFrom(
        string value,
        out Vin result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([Vin](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/vin.md))
