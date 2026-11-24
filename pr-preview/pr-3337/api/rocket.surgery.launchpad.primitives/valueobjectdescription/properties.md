# ValueObjectDescription Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectDescription](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md)
- Kind: `Properties`
- Members: `3`

What an integration needs to know about one value object: the type and the values it holds, in declaration order.

<a id="members"></a>

## Members

- Name: `Members`
- Modifiers: `get; init`
- Returns: [IReadOnlyList<ValueObjectMemberDescription>](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.b3468f8f7f271d2dc63ee1d3333c2bcdd48e93a2/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The values it holds, with their types, in declaration order: what an integration describes the value object with, as its own JSON converter writes it, so the serializer's metadata says nothing of its values.

```csharp
public IReadOnlyList<ValueObjectMemberDescription> Members { get; init; }
```

<a id="properties"></a>

## Properties

- Name: `Properties`
- Modifiers: `get; init`
- Returns: `IReadOnlyList<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.b3468f8f7f271d2dc63ee1d3333c2bcdd48e93a2/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The names of the values it holds.

```csharp
public IReadOnlyList<string> Properties { get; init; }
```

<a id="type"></a>

## Type

- Name: `Type`
- Modifiers: `get; init`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.b3468f8f7f271d2dc63ee1d3333c2bcdd48e93a2/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The value object.

```csharp
public Type Type { get; init; }
```
