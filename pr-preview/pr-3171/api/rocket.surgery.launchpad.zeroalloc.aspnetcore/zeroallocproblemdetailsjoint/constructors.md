# ZeroAllocProblemDetailsJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ZeroAllocProblemDetailsJoint](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocproblemdetailsjoint.md)
- Kind: `Constructors`
- Members: `1`

Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into a 422 with [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md), for controllers and for the exception handler alike. Implements the `Joints.IServiceJoint`

<a id="constructor"></a>

## ZeroAllocProblemDetailsJoint

- Name: `Constructor`

```csharp
public class ZeroAllocProblemDetailsJoint
{
    public ZeroAllocProblemDetailsJoint()
    {
        // ...
    }
}
```
