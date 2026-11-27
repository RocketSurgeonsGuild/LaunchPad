# ValidationFailureProblemDetails Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ValidationFailureProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md)
- Kind: `Properties`
- Members: `1`

The problem details for a request the ZeroAlloc validators rejected, with the failures grouped by property.

<a id="errors"></a>

## Errors

- Name: `Errors`
- Modifiers: `get; init`
- Returns: `IDictionary<string, ValidationFailureDetail[]>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.38910164e414562d3509d65b5a4e0133f1c911cc/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)

The failures, by property name.

```csharp
public IDictionary<string, ValidationFailureDetail[]> Errors { get; init; }
```
