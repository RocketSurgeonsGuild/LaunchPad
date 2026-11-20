# GrpcJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Grpc](/LaunchPad/api/rocket.surgery.launchpad.grpc.md)
- Version: `2.0.1-beta.422`
- Namespace: `Rocket.Surgery.LaunchPad.Grpc.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6f19e0829a4004d537d23cc908a809ddcba9219c/src/Grpc/Joints/GrpcJoint.cs)
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

- [GrpcJoint](/LaunchPad/api/rocket.surgery.launchpad.grpc/grpcjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.grpc/grpcjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
