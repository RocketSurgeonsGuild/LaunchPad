# VoidType Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [VoidType](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.hotchocolate/voidtype.md)
- Kind: `Methods`
- Members: `6`

Represents a void or unit type in Hot Chocolate

<a id="isinstanceoftype"></a>
<a id="isinstanceoftype-ivaluenode"></a>

## IsInstanceOfType(IValueNode)

- Name: `IsInstanceOfType(IValueNode)`
- Modifiers: `override`
- Returns: `bool`

Defines if the specified `valueSyntax` can be parsed by this scalar.

```csharp
public class VoidType
{
    public override bool IsInstanceOfType(
        IValueNode valueSyntax)
    {
        // ...
    }
}
```

## Parameters

- `valueSyntax` (`IValueNode`)
  The literal that shall be checked.

## Returns

`bool` -- `true` if the literal can be parsed by this scalar; otherwise, `false`.

## Exceptions

- `ArgumentNullException` -- `valueSyntax` is `null`.

<a id="parseliteral"></a>
<a id="parseliteral-ivaluenode"></a>

## ParseLiteral(IValueNode)

- Name: `ParseLiteral(IValueNode)`
- Modifiers: `override` `nullable`
- Returns: `object?`

Parses the specified `valueSyntax` to the .NET representation of this type.

```csharp
public class VoidType
{
    public override object? ParseLiteral(
        IValueNode valueSyntax)
    {
        // ...
    }
}
```

## Parameters

- `valueSyntax` (`IValueNode`)
  The literal that shall be parsed.

## Exceptions

- `ArgumentNullException` -- `valueSyntax` is `null`.
- `SerializationException` -- The specified `valueSyntax` cannot be parsed by this scalar.

<a id="parseresult"></a>
<a id="parseresult-object"></a>

## ParseResult(object?)

- Name: `ParseResult(object?)`
- Modifiers: `override`
- Returns: `IValueNode`

Parses a result value of this scalar into a GraphQL value syntax representation.

```csharp
public class VoidType
{
    public override IValueNode ParseResult(
        object? resultValue)
    {
        // ...
    }
}
```

## Parameters

- `resultValue` (`object?`)
  A result value representation of this type.

## Returns

`IValueNode` -- Returns a GraphQL value syntax representation of the `resultValue`.

## Exceptions

- `SerializationException` -- Unable to parse the given `resultValue` into a GraphQL value syntax representation of this type.

<a id="parsevalue"></a>
<a id="parsevalue-object"></a>

## ParseValue(object?)

- Name: `ParseValue(object?)`
- Modifiers: `override`
- Returns: `IValueNode`

Parses the .NET value representation to a value literal.

```csharp
public class VoidType
{
    public override IValueNode ParseValue(
        object? runtimeValue)
    {
        // ...
    }
}
```

## Parameters

- `runtimeValue` (`object?`)
  The .NET value representation.

## Returns

`IValueNode` -- Returns a GraphQL literal representing the .NET value.

## Exceptions

- `SerializationException` -- The specified `runtimeValue` cannot be parsed by this scalar.

<a id="trydeserialize"></a>
<a id="trydeserialize-object-object"></a>

## TryDeserialize(object?, object?)

- Name: `TryDeserialize(object?, object?)`
- Modifiers: `override`
- Returns: `bool`

Tries to deserializes the value from the output format to the .NET value representation.

```csharp
public class VoidType
{
    public override bool TryDeserialize(
        object? resultValue,
        out object? runtimeValue)
    {
        // ...
    }
}
```

## Parameters

- `resultValue` (`object?`)
  The serialized value.
- `runtimeValue` (`object?`)
  The .NET value representation.

## Returns

`bool` -- `true` if the serialized value was correctly deserialized; otherwise, `false`.

<a id="tryserialize"></a>
<a id="tryserialize-object-object"></a>

## TrySerialize(object?, object?)

- Name: `TrySerialize(object?, object?)`
- Modifiers: `override`
- Returns: `bool`

Tries to serializes the .NET value representation to the output format.

```csharp
public class VoidType
{
    public override bool TrySerialize(
        object? runtimeValue,
        out object? resultValue)
    {
        // ...
    }
}
```

## Parameters

- `runtimeValue` (`object?`)
  The .NET value representation.
- `resultValue` (`object?`)
  The serialized value.

## Returns

`bool` -- `true` if the value was correctly serialized; otherwise, `false`.
