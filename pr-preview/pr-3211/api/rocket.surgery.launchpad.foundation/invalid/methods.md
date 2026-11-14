# Invalid Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.foundation.md)
- Type: [Invalid](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.foundation/invalid.md)
- Kind: `Methods`
- Members: `9`

The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Modifiers: `override`
- Returns: [Invalid](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.foundation/invalid.md)

```csharp
public record Invalid
{
    public override Invalid <Clone>$()
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
public record Invalid
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
public record Invalid
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
public record Invalid
{
    public sealed override bool Equals(
        Problem? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Problem?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.foundation/problem.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="equals-invalid"></a>

## Equals(Invalid?)

- Name: `Equals(Invalid?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record Invalid
{
    public bool Equals(
        Invalid? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Invalid?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.foundation/invalid.md))
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
public record Invalid
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
<a id="op-equality-invalid-invalid"></a>

## op_Equality(Invalid?, Invalid?)

- Name: `op_Equality(Invalid?, Invalid?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Invalid
{
    public static bool operator ==(
        Invalid? left,
        Invalid? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Invalid?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.foundation/invalid.md))
- `right` ([Invalid?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.foundation/invalid.md))

<a id="op-inequality"></a>
<a id="op-inequality-invalid-invalid"></a>

## op_Inequality(Invalid?, Invalid?)

- Name: `op_Inequality(Invalid?, Invalid?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Invalid
{
    public static bool operator !=(
        Invalid? left,
        Invalid? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Invalid?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.foundation/invalid.md))
- `right` ([Invalid?](/LaunchPad/pr-preview/pr-3211/api/rocket.surgery.launchpad.foundation/invalid.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record Invalid
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
