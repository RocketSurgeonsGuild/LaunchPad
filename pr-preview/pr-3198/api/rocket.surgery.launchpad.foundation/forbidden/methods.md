# Forbidden Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.foundation.md)
- Type: [Forbidden](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.foundation/forbidden.md)
- Kind: `Methods`
- Members: `9`

The caller may not do this. HTTP 403, gRPC `PermissionDenied`, GraphQL `FORBIDDEN`.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Modifiers: `override`
- Returns: [Forbidden](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.foundation/forbidden.md)

```csharp
public record Forbidden
{
    public override Forbidden <Clone>$()
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
public record Forbidden
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
public record Forbidden
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
public record Forbidden
{
    public sealed override bool Equals(
        Problem? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Problem?](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.foundation/problem.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="equals-forbidden"></a>

## Equals(Forbidden?)

- Name: `Equals(Forbidden?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record Forbidden
{
    public bool Equals(
        Forbidden? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Forbidden?](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.foundation/forbidden.md))
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
public record Forbidden
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
<a id="op-equality-forbidden-forbidden"></a>

## op_Equality(Forbidden?, Forbidden?)

- Name: `op_Equality(Forbidden?, Forbidden?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Forbidden
{
    public static bool operator ==(
        Forbidden? left,
        Forbidden? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Forbidden?](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.foundation/forbidden.md))
- `right` ([Forbidden?](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.foundation/forbidden.md))

<a id="op-inequality"></a>
<a id="op-inequality-forbidden-forbidden"></a>

## op_Inequality(Forbidden?, Forbidden?)

- Name: `op_Inequality(Forbidden?, Forbidden?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Forbidden
{
    public static bool operator !=(
        Forbidden? left,
        Forbidden? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Forbidden?](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.foundation/forbidden.md))
- `right` ([Forbidden?](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.foundation/forbidden.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record Forbidden
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
