# VoidType Properties

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [VoidType](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.hotchocolate/voidtype.md)
- Kind: `Properties`
- Members: `2`

Represents a void or unit type in Hot Chocolate

<a id="runtimetype"></a>

## RuntimeType

- Name: `RuntimeType`
- Modifiers: `override` `get`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3346-merge.Sha.ddf4e8df3db056914fd776aa443a00dc7536dd57/src/HotChocolate/Types/VoidType.cs#L21)

The .NET type representation of this scalar.

```csharp
public override Type RuntimeType { get; }
```

<a id="serializationtype"></a>

## SerializationType

- Name: `SerializationType`
- Modifiers: `override` `get`
- Returns: `ScalarSerializationType`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3346-merge.Sha.ddf4e8df3db056914fd776aa443a00dc7536dd57/src/HotChocolate/Types/VoidType.cs#L24)

Gets the possible types that this scalar can serialize to.

```csharp
public override ScalarSerializationType SerializationType { get; }
```
