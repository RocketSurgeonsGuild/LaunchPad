# GrpcJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Grpc](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.grpc.md)
- Type: [GrpcJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.grpc/grpcjoint.md)
- Kind: `Methods`
- Members: `1`

Registers gRPC. An operation reports a failure with a union response, which the generated gRPC servers answer with its status. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/Grpc/Joints/GrpcJoint.cs#L16)

Register the Service part.

```csharp
public class GrpcJoint
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
