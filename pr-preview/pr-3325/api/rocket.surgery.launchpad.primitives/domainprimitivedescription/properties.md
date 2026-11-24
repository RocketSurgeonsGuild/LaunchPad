# DomainPrimitiveDescription Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitiveDescription](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription.md)
- Kind: `Properties`
- Members: `14`

What an integration needs to know about one domain primitive that holds one value: its value type, the format it is written in, and the constraints of its rules (lengths, patterns, bounds, allowed values). A rule written as code ( `[CustomValidator<T>]`) is not described.

<a id="allowedvalues"></a>

## AllowedValues

- Name: `AllowedValues`
- Modifiers: `nullable` `get; init`
- Returns: `IReadOnlyList<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The only values allowed, in ordinal order, when the rules list them (a smart enum's names).

```csharp
public IReadOnlyList<string>? AllowedValues { get; init; }
```

<a id="format"></a>

## Format

- Name: `Format`
- Modifiers: `nullable` `get; init`
- Returns: `string?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The format of the value (an OpenAPI / JSON Schema format such as `email` or `uri`), when its kind has one.

```csharp
public string? Format { get; init; }
```

<a id="issmartenum"></a>

## IsSmartEnum

- Name: `IsSmartEnum`
- Modifiers: `get; init`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

Whether the domain primitive is a smart enum, whose [DomainPrimitiveDescription.AllowedValues](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#allowedvalues) are its names.

```csharp
public bool IsSmartEnum { get; init; }
```

<a id="isstrongid"></a>

## IsStrongId

- Name: `IsStrongId`
- Modifiers: `get; init`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

Whether the domain primitive is a strong ID.

```csharp
public bool IsStrongId { get; init; }
```

<a id="maximum"></a>

## Maximum

- Name: `Maximum`
- Modifiers: `nullable` `get; init`
- Returns: `string?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The most a value may be, written in the invariant culture.

```csharp
public string? Maximum { get; init; }
```

<a id="maximumisexclusive"></a>

## MaximumIsExclusive

- Name: `MaximumIsExclusive`
- Modifiers: `get; init`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

Whether [DomainPrimitiveDescription.Maximum](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#maximum) itself is excluded.

```csharp
public bool MaximumIsExclusive { get; init; }
```

<a id="maxlength"></a>

## MaxLength

- Name: `MaxLength`
- Modifiers: `nullable` `get; init`
- Returns: `int?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The longest a text value may be.

```csharp
public int? MaxLength { get; init; }
```

<a id="minimum"></a>

## Minimum

- Name: `Minimum`
- Modifiers: `nullable` `get; init`
- Returns: `string?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The least a value may be, written in the invariant culture.

```csharp
public string? Minimum { get; init; }
```

<a id="minimumisexclusive"></a>

## MinimumIsExclusive

- Name: `MinimumIsExclusive`
- Modifiers: `get; init`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

Whether [DomainPrimitiveDescription.Minimum](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#minimum) itself is excluded.

```csharp
public bool MinimumIsExclusive { get; init; }
```

<a id="minlength"></a>

## MinLength

- Name: `MinLength`
- Modifiers: `nullable` `get; init`
- Returns: `int?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The shortest a text value may be.

```csharp
public int? MinLength { get; init; }
```

<a id="patterns"></a>

## Patterns

- Name: `Patterns`
- Modifiers: `get; init`
- Returns: `IReadOnlyList<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The regular expressions a text value must match, every one of them.

```csharp
public IReadOnlyList<string> Patterns { get; init; }
```

<a id="scale"></a>

## Scale

- Name: `Scale`
- Modifiers: `nullable` `get; init`
- Returns: `int?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

The number of digits after the decimal point a decimal value keeps, when its kind sets one.

```csharp
public int? Scale { get; init; }
```

<a id="type"></a>

## Type

- Name: `Type`
- Modifiers: `abstract` `get`
- Returns: `Type`

The domain primitive.

```csharp
public abstract Type Type { get; }
```

<a id="valuetype"></a>

## ValueType

- Name: `ValueType`
- Modifiers: `abstract` `get`
- Returns: `Type`

The value the domain primitive holds, which is what it is written as.

```csharp
public abstract Type ValueType { get; }
```
