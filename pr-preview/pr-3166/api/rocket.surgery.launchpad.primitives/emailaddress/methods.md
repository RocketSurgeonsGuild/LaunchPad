# EmailAddress Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives.md)
- Type: [EmailAddress](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddress.md)
- Kind: `Methods`
- Members: `10`

An email address, valid under [EmailAddressRules](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddressrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class EmailAddress
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-emailaddress"></a>

## CompareTo(EmailAddress?)

- Name: `CompareTo(EmailAddress?)`
- Returns: `int`

```csharp
public sealed class EmailAddress
{
    public int CompareTo(
        EmailAddress? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([EmailAddress?](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddress.md))

<a id="equals"></a>
<a id="equals-emailaddress"></a>

## Equals(EmailAddress?)

- Name: `Equals(EmailAddress?)`
- Returns: `bool`

```csharp
public sealed class EmailAddress
{
    public bool Equals(
        EmailAddress? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([EmailAddress?](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddress.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class EmailAddress
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
- Returns: [EmailAddress](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddress.md)

```csharp
public sealed class EmailAddress
{
    public static EmailAddress From(
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
public sealed class EmailAddress
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
<a id="op-equality-emailaddress-emailaddress"></a>

## op_Equality(EmailAddress?, EmailAddress?)

- Name: `op_Equality(EmailAddress?, EmailAddress?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class EmailAddress
{
    public static bool operator ==(
        EmailAddress? left,
        EmailAddress? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([EmailAddress?](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddress.md))
- `right` ([EmailAddress?](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddress.md))

<a id="op-inequality"></a>
<a id="op-inequality-emailaddress-emailaddress"></a>

## op_Inequality(EmailAddress?, EmailAddress?)

- Name: `op_Inequality(EmailAddress?, EmailAddress?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class EmailAddress
{
    public static bool operator !=(
        EmailAddress? left,
        EmailAddress? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([EmailAddress?](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddress.md))
- `right` ([EmailAddress?](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddress.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class EmailAddress
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
<a id="tryfrom-string-emailaddress"></a>

## TryFrom(string, EmailAddress)

- Name: `TryFrom(string, EmailAddress)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class EmailAddress
{
    public static bool TryFrom(
        string value,
        out EmailAddress result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([EmailAddress](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/emailaddress.md))
