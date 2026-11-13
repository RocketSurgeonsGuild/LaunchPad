# StrongTypeDescription Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives.md)
- Type: [StrongTypeDescription](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/strongtypedescription.md)
- Kind: `Properties`
- Members: `11`

What an integration needs to know about one strong type: its value type, the format it is written in, and the constraints its rules set (lengths, patterns, bounds, allowed values), read from its SuperStrong validators. A rule written as a predicate ( `Satisfies`) is not described.

<a id="allowedvalues"></a>

## AllowedValues

- Name: `AllowedValues`
- Modifiers: `nullable` `get`
- Returns: `IReadOnlyList<string>`

The only values allowed, written in the invariant culture, when the rules list them.

```csharp
public IReadOnlyList<string>? AllowedValues { get; }
```

<a id="format"></a>

## Format

- Name: `Format`
- Modifiers: `nullable` `get`
- Returns: `string?`

The format of the value (an OpenAPI / JSON Schema format such as `email` or `uri`), when the strong type or its rules declare one with [StrongTypeFormatAttribute](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute.md).

```csharp
public string? Format { get; }
```

<a id="maximum"></a>

## Maximum

- Name: `Maximum`
- Modifiers: `nullable` `get`
- Returns: `string?`

The most a number may be, written in the invariant culture.

```csharp
public string? Maximum { get; }
```

<a id="maximumisexclusive"></a>

## MaximumIsExclusive

- Name: `MaximumIsExclusive`
- Modifiers: `get`
- Returns: `bool`

Whether [StrongTypeDescription.Maximum](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#maximum) itself is excluded.

```csharp
public bool MaximumIsExclusive { get; }
```

<a id="maxlength"></a>

## MaxLength

- Name: `MaxLength`
- Modifiers: `nullable` `get`
- Returns: `int?`

The longest a text value may be.

```csharp
public int? MaxLength { get; }
```

<a id="minimum"></a>

## Minimum

- Name: `Minimum`
- Modifiers: `nullable` `get`
- Returns: `string?`

The least a number may be, written in the invariant culture.

```csharp
public string? Minimum { get; }
```

<a id="minimumisexclusive"></a>

## MinimumIsExclusive

- Name: `MinimumIsExclusive`
- Modifiers: `get`
- Returns: `bool`

Whether [StrongTypeDescription.Minimum](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#minimum) itself is excluded.

```csharp
public bool MinimumIsExclusive { get; }
```

<a id="minlength"></a>

## MinLength

- Name: `MinLength`
- Modifiers: `nullable` `get`
- Returns: `int?`

The shortest a text value may be.

```csharp
public int? MinLength { get; }
```

<a id="patterns"></a>

## Patterns

- Name: `Patterns`
- Modifiers: `get`
- Returns: `IReadOnlyList<string>`

The regular expressions a text value must match, every one of them.

```csharp
public IReadOnlyList<string> Patterns { get; }
```

<a id="primitivetype"></a>

## PrimitiveType

- Name: `PrimitiveType`
- Modifiers: `abstract` `get`
- Returns: `Type`

The value the strong type wraps, which is what it is written as.

```csharp
public abstract Type PrimitiveType { get; }
```

<a id="type"></a>

## Type

- Name: `Type`
- Modifiers: `abstract` `get`
- Returns: `Type`

The strong type.

```csharp
public abstract Type Type { get; }
```
