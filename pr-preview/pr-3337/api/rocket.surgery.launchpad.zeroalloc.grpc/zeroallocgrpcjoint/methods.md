# ZeroAllocGrpcJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.zeroalloc.grpc.md)
- Type: [ZeroAllocGrpcJoint](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.zeroalloc.grpc/zeroallocgrpcjoint.md)
- Kind: `Methods`
- Members: `1`

Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into an `StatusCode.InvalidArgument` status. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.7307fcdde362b2224f6c0071b6dfb1eaf21e5cff/src/ZeroAlloc.Grpc/Joints/ZeroAllocGrpcJoint.cs#L30)

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
