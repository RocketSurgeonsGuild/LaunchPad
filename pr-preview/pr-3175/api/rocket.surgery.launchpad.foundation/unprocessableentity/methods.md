# UnprocessableEntity Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.foundation.md)
- Type: [UnprocessableEntity](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.foundation/unprocessableentity.md)
- Kind: `Methods`
- Members: `9`

The request is well formed but fails validation: what FluentValidation reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Modifiers: `override`
- Returns: [UnprocessableEntity](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.foundation/unprocessableentity.md)

```csharp
public record UnprocessableEntity
{
    public override UnprocessableEntity <Clone>$()
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
public record UnprocessableEntity
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
public record UnprocessableEntity
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
public record UnprocessableEntity
{
    public sealed override bool Equals(
        Problem? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Problem?](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.foundation/problem.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="equals-unprocessableentity"></a>

## Equals(UnprocessableEntity?)

- Name: `Equals(UnprocessableEntity?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record UnprocessableEntity
{
    public bool Equals(
        UnprocessableEntity? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([UnprocessableEntity?](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.foundation/unprocessableentity.md))
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
public record UnprocessableEntity
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
<a id="op-equality-unprocessableentity-unprocessableentity"></a>

## op_Equality(UnprocessableEntity?, UnprocessableEntity?)

- Name: `op_Equality(UnprocessableEntity?, UnprocessableEntity?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record UnprocessableEntity
{
    public static bool operator ==(
        UnprocessableEntity? left,
        UnprocessableEntity? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([UnprocessableEntity?](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.foundation/unprocessableentity.md))
- `right` ([UnprocessableEntity?](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.foundation/unprocessableentity.md))

<a id="op-inequality"></a>
<a id="op-inequality-unprocessableentity-unprocessableentity"></a>

## op_Inequality(UnprocessableEntity?, UnprocessableEntity?)

- Name: `op_Inequality(UnprocessableEntity?, UnprocessableEntity?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record UnprocessableEntity
{
    public static bool operator !=(
        UnprocessableEntity? left,
        UnprocessableEntity? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([UnprocessableEntity?](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.foundation/unprocessableentity.md))
- `right` ([UnprocessableEntity?](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.foundation/unprocessableentity.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record UnprocessableEntity
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
