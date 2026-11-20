# ValidationFailureProblemDetails

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Validation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs)
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

- [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails/constructors.md#constructor)

## Properties

- [Errors](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails/properties.md#errors) : `IDictionary<string, ValidationFailureDetail[]>` `get; init` -- The failures, by property name.

## Methods

- [From(ValidationFailedException)](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails/methods.md#from-validationfailedexception) : [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md) `static` -- Creates the problem details for the failures of a `Validation.ValidationFailedException`.
