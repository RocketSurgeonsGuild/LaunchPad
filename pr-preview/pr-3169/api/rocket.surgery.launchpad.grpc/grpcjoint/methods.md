# GrpcJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Grpc](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.grpc.md)
- Type: [GrpcJoint](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.grpc/grpcjoint.md)
- Kind: `Methods`
- Members: `1`

Registers gRPC with the LaunchPad exception interceptors. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

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
