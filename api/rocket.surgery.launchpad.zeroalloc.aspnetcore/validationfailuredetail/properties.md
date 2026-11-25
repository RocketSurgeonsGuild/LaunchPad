# ValidationFailureDetail Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ValidationFailureDetail](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md)
- Kind: `Properties`
- Members: `3`

A single validation failure.

<a id="errorcode"></a>

## ErrorCode

- Name: `ErrorCode`
- Modifiers: `nullable` `get; init`
- Returns: `string?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.752d0f02a7c5d0592531a44cd02f98c0fc107a63/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)

The code of the rule that failed.

```csharp
public string? ErrorCode { get; init; }
```

<a id="errormessage"></a>

## ErrorMessage

- Name: `ErrorMessage`
- Modifiers: `get; init`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.752d0f02a7c5d0592531a44cd02f98c0fc107a63/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)

Why it failed.

```csharp
public string ErrorMessage { get; init; }
```

<a id="propertyname"></a>

## PropertyName

- Name: `PropertyName`
- Modifiers: `get; init`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.752d0f02a7c5d0592531a44cd02f98c0fc107a63/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)

The property that failed.

```csharp
public string PropertyName { get; init; }
```
