# ZeroAllocGrpcJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.grpc.md)
- Type: [ZeroAllocGrpcJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.grpc/zeroallocgrpcjoint.md)
- Kind: `Methods`
- Members: `1`

Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into an `StatusCode.InvalidArgument` status, and the denial of a stream request, which the mediator pipeline throws, into the status of its case. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Grpc/Joints/ZeroAllocGrpcJoint.cs#L32-L38)

Register the Service part.

```csharp
public class ZeroAllocGrpcJoint
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
