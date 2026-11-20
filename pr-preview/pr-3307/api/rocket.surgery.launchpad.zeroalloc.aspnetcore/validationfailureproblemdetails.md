# ValidationFailureProblemDetails

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1258`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Validation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.e1cd1b3273152fe1b84a7e7cf201a85e915d90b0/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)
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

- [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails/constructors.md#constructor)

## Properties

- [Errors](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails/properties.md#errors) : `IDictionary<string, ValidationFailureDetail[]>` `get; init` -- The failures, by property name.

## Methods

- [From(ValidationFailedException)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails/methods.md#from-validationfailedexception) : [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md) `static` -- Creates the problem details for the failures of a `Validation.ValidationFailedException`.
