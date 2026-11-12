# ValidationFailureProblemDetails Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md)
- Kind: `Methods`
- Members: `1`

The problem details for a request the ZeroAlloc validators rejected, with the failures grouped by property.

<a id="from"></a>
<a id="from-validationfailedexception"></a>

## From(ValidationFailedException)

- Name: `From(ValidationFailedException)`
- Modifiers: `static`
- Returns: [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md)

Creates the problem details for the failures of a `Validation.ValidationFailedException`.

```csharp
public class ValidationFailureProblemDetails
{
    public static ValidationFailureProblemDetails From(
        ValidationFailedException exception)
    {
        // ...
    }
}
```

## Parameters

- `exception` (`ValidationFailedException`)
  The exception the mediator threw.
