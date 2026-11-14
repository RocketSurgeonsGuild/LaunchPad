# Slug Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives.md)
- Type: [Slug](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slug.md)
- Kind: `Methods`
- Members: `10`

A URL slug, valid under [SlugRules](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slugrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class Slug
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-slug"></a>

## CompareTo(Slug?)

- Name: `CompareTo(Slug?)`
- Returns: `int`

```csharp
public sealed class Slug
{
    public int CompareTo(
        Slug? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Slug?](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slug.md))

<a id="equals"></a>
<a id="equals-slug"></a>

## Equals(Slug?)

- Name: `Equals(Slug?)`
- Returns: `bool`

```csharp
public sealed class Slug
{
    public bool Equals(
        Slug? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Slug?](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slug.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Slug
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
- Returns: [Slug](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slug.md)

```csharp
public sealed class Slug
{
    public static Slug From(
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
public sealed class Slug
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
<a id="op-equality-slug-slug"></a>

## op_Equality(Slug?, Slug?)

- Name: `op_Equality(Slug?, Slug?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Slug
{
    public static bool operator ==(
        Slug? left,
        Slug? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Slug?](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slug.md))
- `right` ([Slug?](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slug.md))

<a id="op-inequality"></a>
<a id="op-inequality-slug-slug"></a>

## op_Inequality(Slug?, Slug?)

- Name: `op_Inequality(Slug?, Slug?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Slug
{
    public static bool operator !=(
        Slug? left,
        Slug? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Slug?](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slug.md))
- `right` ([Slug?](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slug.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class Slug
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
<a id="tryfrom-string-slug"></a>

## TryFrom(string, Slug)

- Name: `TryFrom(string, Slug)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Slug
{
    public static bool TryFrom(
        string value,
        out Slug result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([Slug](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/slug.md))
