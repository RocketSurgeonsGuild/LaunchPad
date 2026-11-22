# ZeroAllocProblemDetailsJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-beta.447`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.afac89ac2f84551e9433e9fb435d06d6d3ff37f0/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocProblemDetailsJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into a 422 with [ValidationFailureProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md), for controllers and for the exception handler alike. Implements the `Joints.IServiceJoint`

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

- [ZeroAllocProblemDetailsJoint](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocproblemdetailsjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocproblemdetailsjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
- `Joints.ProblemDetailsJoint`
