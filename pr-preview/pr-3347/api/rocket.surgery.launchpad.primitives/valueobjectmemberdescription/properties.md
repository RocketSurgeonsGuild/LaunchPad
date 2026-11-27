# ValueObjectMemberDescription Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectMemberDescription](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md)
- Kind: `Properties`
- Members: `3`

One value a value object holds.

<a id="isnullable"></a>

## IsNullable

- Name: `IsNullable`
- Modifiers: `get; init`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

Whether the value may be missing (a nullable reference or value type).

```csharp
public bool IsNullable { get; init; }
```

<a id="name"></a>

## Name

- Name: `Name`
- Modifiers: `get; init`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The name of its property.

```csharp
public string Name { get; init; }
```

<a id="type"></a>

## Type

- Name: `Type`
- Modifiers: `get; init`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The type of the value.

```csharp
public Type Type { get; init; }
```
