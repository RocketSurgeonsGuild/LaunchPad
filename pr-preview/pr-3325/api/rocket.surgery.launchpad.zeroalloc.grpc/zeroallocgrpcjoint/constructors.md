# ZeroAllocGrpcJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.grpc.md)
- Type: [ZeroAllocGrpcJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.grpc/zeroallocgrpcjoint.md)
- Kind: `Constructors`
- Members: `1`

Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into an `StatusCode.InvalidArgument` status, and the denial of a stream request, which the mediator pipeline throws, into the status of its case. Implements the `Joints.IServiceJoint`

<a id="constructor"></a>

## ZeroAllocGrpcJoint

- Name: `Constructor`

```csharp
public class ZeroAllocGrpcJoint
{
    public ZeroAllocGrpcJoint()
    {
        // ...
    }
}
```
