# DomainPrimitiveDescription<TPrimitive, TValue> Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitiveDescription<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2.md)
- Kind: `Properties`
- Members: `2`

The description of one domain primitive, which knows the domain primitive and its value as generic arguments.

<a id="type"></a>

## Type

- Name: `Type`
- Modifiers: `override` `get`
- Returns: `Type`

The domain primitive.

```csharp
public override Type DomainPrimitiveDescription<TPrimitive, Type { get; }
```

<a id="valuetype"></a>

## ValueType

- Name: `ValueType`
- Modifiers: `override` `get`
- Returns: `Type`

The value the domain primitive holds, which is what it is written as.

```csharp
public override Type DomainPrimitiveDescription<TPrimitive, ValueType { get; }
```
