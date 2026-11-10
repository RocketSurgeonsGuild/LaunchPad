# Conflict Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives.md)
- Type: [Conflict](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/conflict.md)
- Kind: `Methods`
- Members: `9`

The request conflicts with the current state. HTTP 409, gRPC `FailedPrecondition`, GraphQL `CONFLICT`.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Modifiers: `override`
- Returns: [Conflict](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/conflict.md)

```csharp
public record Conflict
{
    public override Conflict <Clone>$()
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
public record Conflict
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
public record Conflict
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
public record Conflict
{
    public sealed override bool Equals(
        Problem? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Problem?](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/problem.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="equals-conflict"></a>

## Equals(Conflict?)

- Name: `Equals(Conflict?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record Conflict
{
    public bool Equals(
        Conflict? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Conflict?](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/conflict.md))
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
public record Conflict
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
<a id="op-equality-conflict-conflict"></a>

## op_Equality(Conflict?, Conflict?)

- Name: `op_Equality(Conflict?, Conflict?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Conflict
{
    public static bool operator ==(
        Conflict? left,
        Conflict? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Conflict?](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/conflict.md))
- `right` ([Conflict?](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/conflict.md))

<a id="op-inequality"></a>
<a id="op-inequality-conflict-conflict"></a>

## op_Inequality(Conflict?, Conflict?)

- Name: `op_Inequality(Conflict?, Conflict?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Conflict
{
    public static bool operator !=(
        Conflict? left,
        Conflict? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Conflict?](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/conflict.md))
- `right` ([Conflict?](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.primitives/conflict.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record Conflict
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
