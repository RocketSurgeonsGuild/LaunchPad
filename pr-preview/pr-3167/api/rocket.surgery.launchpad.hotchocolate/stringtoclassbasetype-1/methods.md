# StringToClassBaseType<TRuntimeType> Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [StringToClassBaseType<TRuntimeType>](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.hotchocolate/stringtoclassbasetype-1.md)
- Kind: `Methods`
- Members: `3`

String class base type

<a id="parseresult"></a>
<a id="parseresult-object"></a>

## ParseResult(object?)

- Name: `ParseResult(object?)`
- Modifiers: `override`
- Returns: `IValueNode`

Parses a result value of this scalar into a GraphQL value syntax representation.

```csharp
public abstract class StringToClassBaseType<TRuntimeType>
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

<a id="trydeserialize"></a>
<a id="trydeserialize-object-object"></a>

## TryDeserialize(object?, object?)

- Name: `TryDeserialize(object?, object?)`
- Modifiers: `override`
- Returns: `bool`

Tries to deserializes the value from the output format to the .NET value representation.

```csharp
public abstract class StringToClassBaseType<TRuntimeType>
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
public abstract class StringToClassBaseType<TRuntimeType>
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
