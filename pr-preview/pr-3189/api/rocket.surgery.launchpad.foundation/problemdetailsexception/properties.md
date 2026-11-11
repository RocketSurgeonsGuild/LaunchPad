# ProblemDetailsException Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation.md)
- Type: [ProblemDetailsException](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/problemdetailsexception.md)
- Kind: `Properties`
- Members: `4`

NotFoundException.

<a id="instance"></a>

## Instance

- Name: `Instance`
- Modifiers: `nullable` `get; init`
- Returns: `string?`

The instance for the request

```csharp
public string? Instance { get; init; }
```

<a id="link"></a>

## Link

- Name: `Link`
- Modifiers: `nullable` `get; init`
- Returns: `string?`

Request Type

```csharp
public string? Link { get; init; }
```

<a id="properties"></a>

## Properties

- Name: `Properties`
- Modifiers: `get; init`
- Returns: `IDictionary<string, object?>`

Additional properties

```csharp
public IDictionary<string, object?> Properties { get; init; }
```

<a id="title"></a>

## Title

- Name: `Title`
- Modifiers: `nullable` `get; init`
- Returns: `string?`

Request title

```csharp
public string? Title { get; init; }
```
