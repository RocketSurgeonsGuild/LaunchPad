# PhoneNumber Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [PhoneNumber](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumber.md)
- Kind: `Methods`
- Members: `10`

A phone number in E.164 form, valid under [PhoneNumberRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumberrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class PhoneNumber
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-phonenumber"></a>

## CompareTo(PhoneNumber?)

- Name: `CompareTo(PhoneNumber?)`
- Returns: `int`

```csharp
public sealed class PhoneNumber
{
    public int CompareTo(
        PhoneNumber? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([PhoneNumber?](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumber.md))

<a id="equals"></a>
<a id="equals-phonenumber"></a>

## Equals(PhoneNumber?)

- Name: `Equals(PhoneNumber?)`
- Returns: `bool`

```csharp
public sealed class PhoneNumber
{
    public bool Equals(
        PhoneNumber? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([PhoneNumber?](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumber.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class PhoneNumber
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
- Returns: [PhoneNumber](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumber.md)

```csharp
public sealed class PhoneNumber
{
    public static PhoneNumber From(
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
public sealed class PhoneNumber
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
<a id="op-equality-phonenumber-phonenumber"></a>

## op_Equality(PhoneNumber?, PhoneNumber?)

- Name: `op_Equality(PhoneNumber?, PhoneNumber?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class PhoneNumber
{
    public static bool operator ==(
        PhoneNumber? left,
        PhoneNumber? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([PhoneNumber?](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumber.md))
- `right` ([PhoneNumber?](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumber.md))

<a id="op-inequality"></a>
<a id="op-inequality-phonenumber-phonenumber"></a>

## op_Inequality(PhoneNumber?, PhoneNumber?)

- Name: `op_Inequality(PhoneNumber?, PhoneNumber?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class PhoneNumber
{
    public static bool operator !=(
        PhoneNumber? left,
        PhoneNumber? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([PhoneNumber?](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumber.md))
- `right` ([PhoneNumber?](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumber.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class PhoneNumber
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
<a id="tryfrom-string-phonenumber"></a>

## TryFrom(string, PhoneNumber)

- Name: `TryFrom(string, PhoneNumber)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class PhoneNumber
{
    public static bool TryFrom(
        string value,
        out PhoneNumber result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([PhoneNumber](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumber.md))
