# VoidType Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [VoidType](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.hotchocolate/voidtype.md)
- Kind: `Methods`
- Members: `6`

Represents a void or unit type in Hot Chocolate

<a id="coerceinputliteral"></a>
<a id="coerceinputliteral-ivaluenode"></a>

## CoerceInputLiteral(IValueNode)

- Name: `CoerceInputLiteral(IValueNode)`
- Modifiers: `override`
- Returns: `object`

Coerces a GraphQL literal (AST value node) into a runtime value.

```csharp
public class VoidType
{
    public override object CoerceInputLiteral(
        IValueNode valueLiteral)
    {
        // ...
    }
}
```

## Parameters

- `valueLiteral` (`IValueNode`)
  The GraphQL literal to coerce.

## Returns

`object` -- Returns the runtime value representation.

## Exceptions

- `ArgumentNullException` -- `valueLiteral` is `null`.
- `LeafCoercionException` -- Unable to coerce the given `valueLiteral` into a runtime value.

<a id="coerceinputvalue"></a>
<a id="coerceinputvalue-jsonelement-ifeatureprovider"></a>

## CoerceInputValue(JsonElement, IFeatureProvider)

- Name: `CoerceInputValue(JsonElement, IFeatureProvider)`
- Modifiers: `override`
- Returns: `object`

Coerces an external input value (deserialized JSON) into a runtime value.

```csharp
public class VoidType
{
    public override object CoerceInputValue(
        JsonElement inputValue,
        IFeatureProvider context)
    {
        // ...
    }
}
```

## Parameters

- `inputValue` (`JsonElement`)
  The deserialized JSON input value to coerce.
- `context` (`IFeatureProvider`)
  Provides access to the coercion context.

## Returns

`object` -- Returns the runtime value representation.

## Exceptions

- `LeafCoercionException` -- Unable to coerce the given `inputValue` into a runtime value.

<a id="coerceoutputvalue"></a>
<a id="coerceoutputvalue-object-resultelement"></a>

## CoerceOutputValue(object, ResultElement)

- Name: `CoerceOutputValue(object, ResultElement)`
- Modifiers: `override`

Coerces a runtime value into an external output representation and writes it to the result.

```csharp
public class VoidType
{
    public override void CoerceOutputValue(
        object runtimeValue,
        ResultElement resultValue)
    {
        // ...
    }
}
```

## Parameters

- `runtimeValue` (`object`)
  The runtime value to coerce. Must not be `null`; null handling is the responsibility of the caller.
- `resultValue` (`ResultElement`)
  The result element to write the output value to.

## Exceptions

- `ArgumentNullException` -- `runtimeValue` is `null`.
- `LeafCoercionException` -- Unable to coerce the given `runtimeValue` into an output value.

<a id="isvaluecompatible"></a>
<a id="isvaluecompatible-ivaluenode"></a>

## IsValueCompatible(IValueNode)

- Name: `IsValueCompatible(IValueNode)`
- Modifiers: `override`
- Returns: `bool`

Checks if the value literal is compatible with this scalar type.

```csharp
public class VoidType
{
    public override bool IsValueCompatible(
        IValueNode valueLiteral)
    {
        // ...
    }
}
```

## Parameters

- `valueLiteral` (`IValueNode`)
  The value literal to check.

## Returns

`bool` -- `true` if the value literal is compatible with this type; otherwise, `false`.

<a id="isvaluecompatible-jsonelement"></a>

## IsValueCompatible(JsonElement)

- Name: `IsValueCompatible(JsonElement)`
- Modifiers: `override`
- Returns: `bool`

Determines if the given `inputValue` is compatible with this type.

```csharp
public class VoidType
{
    public override bool IsValueCompatible(
        JsonElement inputValue)
    {
        // ...
    }
}
```

## Parameters

- `inputValue` (`JsonElement`)
  The deserialized JSON input value to validate.

## Returns

`bool` -- `true` if the given `inputValue` is compatible with this type.

<a id="valuetoliteral"></a>
<a id="valuetoliteral-object"></a>

## ValueToLiteral(object)

- Name: `ValueToLiteral(object)`
- Modifiers: `override`
- Returns: `IValueNode`

Converts a runtime value into a GraphQL literal (AST value node). Used for default value representation in SDL and introspection.

```csharp
public class VoidType
{
    public override IValueNode ValueToLiteral(
        object runtimeValue)
    {
        // ...
    }
}
```

## Parameters

- `runtimeValue` (`object`)
  The runtime value to convert. Must not be `null`; null handling is the responsibility of the caller.

## Returns

`IValueNode` -- Returns a GraphQL literal representation of the runtime value.

## Exceptions

- `ArgumentNullException` -- `runtimeValue` is `null`.
- `LeafCoercionException` -- Unable to convert the given `runtimeValue` into a literal.
