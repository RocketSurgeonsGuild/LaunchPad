# ZeroAllocProblemDetailsJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3334-merge.Sha.1bdca6582f82d4de4d3840709c4a6fa63d4c5f0c/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocProblemDetailsJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into a 422 with [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md), for controllers and for the exception handler alike. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class ZeroAllocProblemDetailsJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Runs after the `Joints.ProblemDetailsJoint`, so the status code selector it configures is kept for every other exception.

## Constructors

- [ZeroAllocProblemDetailsJoint](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocproblemdetailsjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocproblemdetailsjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
- `Joints.ProblemDetailsJoint`
