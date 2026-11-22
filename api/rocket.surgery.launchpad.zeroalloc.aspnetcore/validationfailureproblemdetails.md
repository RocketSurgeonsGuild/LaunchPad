# ValidationFailureProblemDetails

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-beta.444`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Validation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)
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
