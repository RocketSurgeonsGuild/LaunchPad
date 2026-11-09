# IProblemDetailsData Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3156/api/rocket.surgery.launchpad.primitives.md)
- Type: [IProblemDetailsData](/LaunchPad/pr-preview/pr-3156/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata.md)
- Kind: `Properties`
- Members: `4`

A common interface used for definiting problems

<a id="instance"></a>

## Instance

- Name: `Instance`
- Modifiers: `abstract` `nullable` `get`
- Returns: `string?`

The instance for the request

```csharp
public abstract string? Instance { get; }
```

<a id="link"></a>

## Link

- Name: `Link`
- Modifiers: `abstract` `nullable` `get`
- Returns: `string?`

Request Type

```csharp
public abstract string? Link { get; }
```

<a id="properties"></a>

## Properties

- Name: `Properties`
- Modifiers: `abstract` `get`
- Returns: `IDictionary<string, object?>`

Additional properties

```csharp
public abstract IDictionary<string, object?> Properties { get; }
```

<a id="title"></a>

## Title

- Name: `Title`
- Modifiers: `abstract` `nullable` `get`
- Returns: `string?`

Request title

```csharp
public abstract string? Title { get; }
```
