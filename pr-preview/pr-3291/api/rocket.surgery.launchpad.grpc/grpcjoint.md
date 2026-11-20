# GrpcJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Grpc](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.grpc.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.Grpc.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3291-merge.Sha.13e74febbd2cc111b9b844ce764f2522bbd95912/src/Grpc/Joints/GrpcJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Registers gRPC. An operation reports a failure with a union response, which the generated gRPC servers answer with its status. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Grpc.Joints;

public class GrpcJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [GrpcJoint](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.grpc/grpcjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.grpc/grpcjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
