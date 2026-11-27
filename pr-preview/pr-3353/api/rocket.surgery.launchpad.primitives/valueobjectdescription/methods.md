# ValueObjectDescription Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3353/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectDescription](/LaunchPad/pr-preview/pr-3353/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md)
- Kind: `Methods`
- Members: `8`

What an integration needs to know about one value object: the type and the values it holds, in declaration order.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Returns: [ValueObjectDescription](/LaunchPad/pr-preview/pr-3353/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md)

```csharp
public record ValueObjectDescription
{
    public ValueObjectDescription <Clone>$()
    {
        // ...
    }
}
```

<a id="deconstruct"></a>
<a id="deconstruct-type-ireadonlylist-string"></a>

## Deconstruct(Type, IReadOnlyList<string>)

- Name: `Deconstruct(Type, IReadOnlyList<string>)`

```csharp
public record ValueObjectDescription
{
    public void Deconstruct(
        out Type Type,
        out IReadOnlyList<string> Properties)
    {
        // ...
    }
}
```

## Parameters

- `Type` (`Type`)
- `Properties` (`IReadOnlyList<string>`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record ValueObjectDescription
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

<a id="equals-valueobjectdescription"></a>

## Equals(ValueObjectDescription?)

- Name: `Equals(ValueObjectDescription?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record ValueObjectDescription
{
    public bool Equals(
        ValueObjectDescription? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([ValueObjectDescription?](/LaunchPad/pr-preview/pr-3353/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md))
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
public record ValueObjectDescription
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
<a id="op-equality-valueobjectdescription-valueobjectdescription"></a>

## op_Equality(ValueObjectDescription?, ValueObjectDescription?)

- Name: `op_Equality(ValueObjectDescription?, ValueObjectDescription?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record ValueObjectDescription
{
    public static bool operator ==(
        ValueObjectDescription? left,
        ValueObjectDescription? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([ValueObjectDescription?](/LaunchPad/pr-preview/pr-3353/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md))
- `right` ([ValueObjectDescription?](/LaunchPad/pr-preview/pr-3353/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md))

<a id="op-inequality"></a>
<a id="op-inequality-valueobjectdescription-valueobjectdescription"></a>

## op_Inequality(ValueObjectDescription?, ValueObjectDescription?)

- Name: `op_Inequality(ValueObjectDescription?, ValueObjectDescription?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record ValueObjectDescription
{
    public static bool operator !=(
        ValueObjectDescription? left,
        ValueObjectDescription? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([ValueObjectDescription?](/LaunchPad/pr-preview/pr-3353/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md))
- `right` ([ValueObjectDescription?](/LaunchPad/pr-preview/pr-3353/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record ValueObjectDescription
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
