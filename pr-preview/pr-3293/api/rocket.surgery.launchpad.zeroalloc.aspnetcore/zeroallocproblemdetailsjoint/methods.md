# ZeroAllocProblemDetailsJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3293/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ZeroAllocProblemDetailsJoint](/LaunchPad/pr-preview/pr-3293/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocproblemdetailsjoint.md)
- Kind: `Methods`
- Members: `1`

Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into a 422 with [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3293/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md), for controllers and for the exception handler alike. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3293-merge.Sha.4c1c9433962eeb0eff759138d3819e6ad0cb6311/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocProblemDetailsJoint.cs#L32-L44)

Register the Service part.

```csharp
public class ZeroAllocProblemDetailsJoint
{
    public void Register(
        IDovetailContext context,
        IServiceCollection services)
    {
        // ...
    }
}
```

## Parameters

- `context` (`IDovetailContext`)
- `services` (`IServiceCollection`)
