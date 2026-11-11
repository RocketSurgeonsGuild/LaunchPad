# StrongTypeDescription<TStrongType, TPrimitive> Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives.md)
- Type: [StrongTypeDescription<TStrongType, TPrimitive>](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.primitives/strongtypedescription-2.md)
- Kind: `Properties`
- Members: `2`

The description of one strong type, which knows the strong type and its value as generic arguments.

<a id="primitivetype"></a>

## PrimitiveType

- Name: `PrimitiveType`
- Modifiers: `override` `get`
- Returns: `Type`

The value the strong type wraps, which is what it is written as.

```csharp
public override Type StrongTypeDescription<TStrongType, PrimitiveType { get; }
```

<a id="type"></a>

## Type

- Name: `Type`
- Modifiers: `override` `get`
- Returns: `Type`

The strong type.

```csharp
public override Type StrongTypeDescription<TStrongType, Type { get; }
```
