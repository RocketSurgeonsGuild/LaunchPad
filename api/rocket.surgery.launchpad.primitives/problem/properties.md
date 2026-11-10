# Problem Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [Problem](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem.md)
- Kind: `Properties`
- Members: `5`

The base of LaunchPad's standard error cases: what a handler returns, as a case of its result union, instead of throwing. Each transport turns a case into its own error shape (HTTP ProblemDetails, a gRPC status, a GraphQL error) from [IProblemDetailsData](/LaunchPad/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata.md).

<a id="detail"></a>

## Detail

- Name: `Detail`
- Modifiers: `nullable` `get; init`
- Returns: `string?`

A human-readable explanation specific to this occurrence of the problem.

```csharp
public string? Detail { get; init; }
```

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
