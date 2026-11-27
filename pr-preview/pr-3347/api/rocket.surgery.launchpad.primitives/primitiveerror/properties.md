# PrimitiveError Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives.md)
- Type: [PrimitiveError](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/primitiveerror.md)
- Kind: `Properties`
- Members: `4`

Why a domain primitive rejected a value: a code ( `LENGTH`, `FORMAT`, `RANGE`, ...) and a message.

<a id="code"></a>

## Code

- Name: `Code`
- Modifiers: `nullable` `get; init`
- Returns: `string?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.4aab33c5f701c0001e99ba64a669e730647ab45f/src/Primitives/PrimitiveError.cs)

The code, or `null` when there is no error.

```csharp
public string? Code { get; init; }
```

<a id="iserror"></a>

## IsError

- Name: `IsError`
- Modifiers: `get`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.4aab33c5f701c0001e99ba64a669e730647ab45f/src/Primitives/PrimitiveError.cs#L19)

Whether this is an error.

```csharp
public bool IsError { get; }
```

<a id="message"></a>

## Message

- Name: `Message`
- Modifiers: `nullable` `get; init`
- Returns: `string?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.4aab33c5f701c0001e99ba64a669e730647ab45f/src/Primitives/PrimitiveError.cs)

The message.

```csharp
public string? Message { get; init; }
```

<a id="none"></a>

## None

- Name: `None`
- Modifiers: `static` `get`
- Returns: [PrimitiveError](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/primitiveerror.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.4aab33c5f701c0001e99ba64a669e730647ab45f/src/Primitives/PrimitiveError.cs#L14)

No error.

```csharp
public static PrimitiveError None { get; }
```
