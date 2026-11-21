# ValidationFailureDetail Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ValidationFailureDetail](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md)
- Kind: `Properties`
- Members: `3`

A single validation failure.

<a id="errorcode"></a>

## ErrorCode

- Name: `ErrorCode`
- Modifiers: `nullable` `get; init`
- Returns: `string?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3310-merge.Sha.8860010d83b5d02194514237744998b27c36d06d/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)

The code of the rule that failed.

```csharp
public string? ErrorCode { get; init; }
```

<a id="errormessage"></a>

## ErrorMessage

- Name: `ErrorMessage`
- Modifiers: `get; init`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3310-merge.Sha.8860010d83b5d02194514237744998b27c36d06d/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)

Why it failed.

```csharp
public string ErrorMessage { get; init; }
```

<a id="propertyname"></a>

## PropertyName

- Name: `PropertyName`
- Modifiers: `get; init`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3310-merge.Sha.8860010d83b5d02194514237744998b27c36d06d/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)

The property that failed.

```csharp
public string PropertyName { get; init; }
```
