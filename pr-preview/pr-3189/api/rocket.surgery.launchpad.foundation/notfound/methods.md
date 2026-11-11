# NotFound Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation.md)
- Type: [NotFound](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/notfound.md)
- Kind: `Methods`
- Members: `9`

The requested entity doesn't exist. HTTP 404, gRPC `NotFound`, GraphQL `NOT_FOUND`.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Modifiers: `override`
- Returns: [NotFound](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/notfound.md)

```csharp
public record NotFound
{
    public override NotFound <Clone>$()
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
public record NotFound
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
public record NotFound
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
public record NotFound
{
    public sealed override bool Equals(
        Problem? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Problem?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/problem.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="equals-notfound"></a>

## Equals(NotFound?)

- Name: `Equals(NotFound?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record NotFound
{
    public bool Equals(
        NotFound? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([NotFound?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/notfound.md))
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
public record NotFound
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
<a id="op-equality-notfound-notfound"></a>

## op_Equality(NotFound?, NotFound?)

- Name: `op_Equality(NotFound?, NotFound?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record NotFound
{
    public static bool operator ==(
        NotFound? left,
        NotFound? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([NotFound?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/notfound.md))
- `right` ([NotFound?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/notfound.md))

<a id="op-inequality"></a>
<a id="op-inequality-notfound-notfound"></a>

## op_Inequality(NotFound?, NotFound?)

- Name: `op_Inequality(NotFound?, NotFound?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record NotFound
{
    public static bool operator !=(
        NotFound? left,
        NotFound? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([NotFound?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/notfound.md))
- `right` ([NotFound?](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/notfound.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record NotFound
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
