# ZeroAllocGrpcJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.zeroalloc.grpc.md)
- Type: [ZeroAllocGrpcJoint](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.zeroalloc.grpc/zeroallocgrpcjoint.md)
- Kind: `Methods`
- Members: `1`

Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into an `StatusCode.InvalidArgument` status. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3291-merge.Sha.13e74febbd2cc111b9b844ce764f2522bbd95912/src/ZeroAlloc.Grpc/Joints/ZeroAllocGrpcJoint.cs#L30)

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
