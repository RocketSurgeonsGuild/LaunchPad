# ZeroAllocGrpcJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.grpc.md)
- Version: `2.0.1-pr.1259`
- Namespace: `Rocket.Surgery.LaunchPad.Grpc.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.0d6c4ca8b5b3c5055adee085d785c5712bbc3314/src/ZeroAlloc.Grpc/Joints/ZeroAllocGrpcJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into an `StatusCode.InvalidArgument` status. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Grpc.Joints;

public class ZeroAllocGrpcJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Runs after the `Joints.GrpcJoint`, so its interceptor is added after, and so runs inside, the exception interceptors that joint adds: the failure is already an `Core.RpcException` when they see it.

## Constructors

- [ZeroAllocGrpcJoint](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.grpc/zeroallocgrpcjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.grpc/zeroallocgrpcjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
- `Joints.GrpcJoint`
