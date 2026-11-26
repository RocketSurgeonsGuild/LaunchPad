# ValueObjectMemberDescription Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectMemberDescription](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md)
- Kind: `Methods`
- Members: `8`

One value a value object holds.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Returns: [ValueObjectMemberDescription](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md)

```csharp
public record ValueObjectMemberDescription
{
    public ValueObjectMemberDescription <Clone>$()
    {
        // ...
    }
}
```

<a id="deconstruct"></a>
<a id="deconstruct-string-type-bool"></a>

## Deconstruct(string, Type, bool)

- Name: `Deconstruct(string, Type, bool)`

```csharp
public record ValueObjectMemberDescription
{
    public void Deconstruct(
        out string Name,
        out Type Type,
        out bool IsNullable)
    {
        // ...
    }
}
```

## Parameters

- `Name` (`string`)
- `Type` (`Type`)
- `IsNullable` (`bool`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record ValueObjectMemberDescription
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

<a id="equals-valueobjectmemberdescription"></a>

## Equals(ValueObjectMemberDescription?)

- Name: `Equals(ValueObjectMemberDescription?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record ValueObjectMemberDescription
{
    public bool Equals(
        ValueObjectMemberDescription? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([ValueObjectMemberDescription?](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md))
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
public record ValueObjectMemberDescription
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
<a id="op-equality-valueobjectmemberdescription-valueobjectmemberdescription"></a>

## op_Equality(ValueObjectMemberDescription?, ValueObjectMemberDescription?)

- Name: `op_Equality(ValueObjectMemberDescription?, ValueObjectMemberDescription?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record ValueObjectMemberDescription
{
    public static bool operator ==(
        ValueObjectMemberDescription? left,
        ValueObjectMemberDescription? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([ValueObjectMemberDescription?](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md))
- `right` ([ValueObjectMemberDescription?](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md))

<a id="op-inequality"></a>
<a id="op-inequality-valueobjectmemberdescription-valueobjectmemberdescription"></a>

## op_Inequality(ValueObjectMemberDescription?, ValueObjectMemberDescription?)

- Name: `op_Inequality(ValueObjectMemberDescription?, ValueObjectMemberDescription?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record ValueObjectMemberDescription
{
    public static bool operator !=(
        ValueObjectMemberDescription? left,
        ValueObjectMemberDescription? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([ValueObjectMemberDescription?](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md))
- `right` ([ValueObjectMemberDescription?](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record ValueObjectMemberDescription
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
