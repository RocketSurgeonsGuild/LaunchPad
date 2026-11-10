# Unauthorized Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives.md)
- Type: [Unauthorized](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/unauthorized.md)
- Kind: `Methods`
- Members: `9`

The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Modifiers: `override`
- Returns: [Unauthorized](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/unauthorized.md)

```csharp
public record Unauthorized
{
    public override Unauthorized <Clone>$()
    {
        // ...
    }
}
```

<a id="deconstruct"></a>
<a id="deconstruct-string"></a>

## Deconstruct(string?)

- Name: `Deconstruct(string?)`

```csharp
public record Unauthorized
{
    public void Deconstruct(
        out string? Detail)
    {
        // ...
    }
}
```

## Parameters

- `Detail` (`string?`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record Unauthorized
{
    public override bool Equals(
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

<a id="equals-problem"></a>

## Equals(Problem?)

- Name: `Equals(Problem?)`
- Modifiers: `override`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record Unauthorized
{
    public sealed override bool Equals(
        Problem? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Problem?](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/problem.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="equals-unauthorized"></a>

## Equals(Unauthorized?)

- Name: `Equals(Unauthorized?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record Unauthorized
{
    public bool Equals(
        Unauthorized? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Unauthorized?](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/unauthorized.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public record Unauthorized
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
<a id="op-equality-unauthorized-unauthorized"></a>

## op_Equality(Unauthorized?, Unauthorized?)

- Name: `op_Equality(Unauthorized?, Unauthorized?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Unauthorized
{
    public static bool operator ==(
        Unauthorized? left,
        Unauthorized? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Unauthorized?](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/unauthorized.md))
- `right` ([Unauthorized?](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/unauthorized.md))

<a id="op-inequality"></a>
<a id="op-inequality-unauthorized-unauthorized"></a>

## op_Inequality(Unauthorized?, Unauthorized?)

- Name: `op_Inequality(Unauthorized?, Unauthorized?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Unauthorized
{
    public static bool operator !=(
        Unauthorized? left,
        Unauthorized? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Unauthorized?](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/unauthorized.md))
- `right` ([Unauthorized?](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/unauthorized.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record Unauthorized
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
