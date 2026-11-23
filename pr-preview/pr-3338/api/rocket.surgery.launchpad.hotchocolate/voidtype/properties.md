# VoidType Properties

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3338/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [VoidType](/LaunchPad/pr-preview/pr-3338/api/rocket.surgery.launchpad.hotchocolate/voidtype.md)
- Kind: `Properties`
- Members: `2`

Represents a void or unit type in Hot Chocolate

<a id="runtimetype"></a>

## RuntimeType

- Name: `RuntimeType`
- Modifiers: `override` `get`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3338-merge.Sha.3cb04ef79b1e670b9474115cbea11b9f679be3f4/src/HotChocolate/Types/VoidType.cs#L21)

The .NET type representation of this scalar.

```csharp
public override Type RuntimeType { get; }
```

<a id="serializationtype"></a>

## SerializationType

- Name: `SerializationType`
- Modifiers: `override` `get`
- Returns: `ScalarSerializationType`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3338-merge.Sha.3cb04ef79b1e670b9474115cbea11b9f679be3f4/src/HotChocolate/Types/VoidType.cs#L24)

Gets the possible types that this scalar can serialize to.

```csharp
public override ScalarSerializationType SerializationType { get; }
```
