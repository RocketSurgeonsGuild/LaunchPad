# ValidationFailureProblemDetails

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-beta.327`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Validation`
- Target framework: `net10.0`
- Inherits: `ProblemDetails`

The problem details for a request the ZeroAlloc validators rejected, with the failures grouped by property.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

public class ValidationFailureProblemDetails
    : Microsoft.AspNetCore.Mvc.ProblemDetails
{
    // ...
}
```

## Constructors

- [ValidationFailureProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails/constructors.md#constructor)

## Properties

- [Errors](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails/properties.md#errors) : `IDictionary<string, ValidationFailureDetail[]>` `get; init` -- The failures, by property name.

## Methods

- [From(ValidationFailedException)](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails/methods.md#from-validationfailedexception) : [ValidationFailureProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md) `static` -- Creates the problem details for the failures of a `Validation.ValidationFailedException`.
