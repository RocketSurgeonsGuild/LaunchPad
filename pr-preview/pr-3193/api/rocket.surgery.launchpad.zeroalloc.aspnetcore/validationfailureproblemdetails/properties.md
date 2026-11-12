# ValidationFailureProblemDetails Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md)
- Kind: `Properties`
- Members: `1`

The problem details for a request the ZeroAlloc validators rejected, with the failures grouped by property.

<a id="errors"></a>

## Errors

- Name: `Errors`
- Modifiers: `get; init`
- Returns: `IDictionary<string, ValidationFailureDetail[]>`

The failures, by property name.

```csharp
public IDictionary<string, ValidationFailureDetail[]> Errors { get; init; }
```
