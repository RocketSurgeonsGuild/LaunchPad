# Iban Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives.md)
- Type: [Iban](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/iban.md)
- Kind: `Methods`
- Members: `10`

An IBAN in electronic format, valid under [IbanRules](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/ibanrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class Iban
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-iban"></a>

## CompareTo(Iban?)

- Name: `CompareTo(Iban?)`
- Returns: `int`

```csharp
public sealed class Iban
{
    public int CompareTo(
        Iban? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Iban?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/iban.md))

<a id="equals"></a>
<a id="equals-iban"></a>

## Equals(Iban?)

- Name: `Equals(Iban?)`
- Returns: `bool`

```csharp
public sealed class Iban
{
    public bool Equals(
        Iban? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Iban?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/iban.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Iban
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
- Returns: [Iban](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/iban.md)

```csharp
public sealed class Iban
{
    public static Iban From(
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
public sealed class Iban
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
<a id="op-equality-iban-iban"></a>

## op_Equality(Iban?, Iban?)

- Name: `op_Equality(Iban?, Iban?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Iban
{
    public static bool operator ==(
        Iban? left,
        Iban? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Iban?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/iban.md))
- `right` ([Iban?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/iban.md))

<a id="op-inequality"></a>
<a id="op-inequality-iban-iban"></a>

## op_Inequality(Iban?, Iban?)

- Name: `op_Inequality(Iban?, Iban?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Iban
{
    public static bool operator !=(
        Iban? left,
        Iban? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Iban?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/iban.md))
- `right` ([Iban?](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/iban.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class Iban
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
<a id="tryfrom-string-iban"></a>

## TryFrom(string, Iban)

- Name: `TryFrom(string, Iban)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Iban
{
    public static bool TryFrom(
        string value,
        out Iban result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([Iban](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/iban.md))
