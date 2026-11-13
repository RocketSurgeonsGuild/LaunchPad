# Isbn Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives.md)
- Type: [Isbn](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbn.md)
- Kind: `Methods`
- Members: `10`

An ISBN-10 or ISBN-13, valid under [IsbnRules](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbnrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class Isbn
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-isbn"></a>

## CompareTo(Isbn?)

- Name: `CompareTo(Isbn?)`
- Returns: `int`

```csharp
public sealed class Isbn
{
    public int CompareTo(
        Isbn? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Isbn?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbn.md))

<a id="equals"></a>
<a id="equals-isbn"></a>

## Equals(Isbn?)

- Name: `Equals(Isbn?)`
- Returns: `bool`

```csharp
public sealed class Isbn
{
    public bool Equals(
        Isbn? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Isbn?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbn.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Isbn
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
- Returns: [Isbn](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbn.md)

```csharp
public sealed class Isbn
{
    public static Isbn From(
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
public sealed class Isbn
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
<a id="op-equality-isbn-isbn"></a>

## op_Equality(Isbn?, Isbn?)

- Name: `op_Equality(Isbn?, Isbn?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Isbn
{
    public static bool operator ==(
        Isbn? left,
        Isbn? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Isbn?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbn.md))
- `right` ([Isbn?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbn.md))

<a id="op-inequality"></a>
<a id="op-inequality-isbn-isbn"></a>

## op_Inequality(Isbn?, Isbn?)

- Name: `op_Inequality(Isbn?, Isbn?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Isbn
{
    public static bool operator !=(
        Isbn? left,
        Isbn? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Isbn?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbn.md))
- `right` ([Isbn?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbn.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class Isbn
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
<a id="tryfrom-string-isbn"></a>

## TryFrom(string, Isbn)

- Name: `TryFrom(string, Isbn)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Isbn
{
    public static bool TryFrom(
        string value,
        out Isbn result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([Isbn](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.primitives/isbn.md))
