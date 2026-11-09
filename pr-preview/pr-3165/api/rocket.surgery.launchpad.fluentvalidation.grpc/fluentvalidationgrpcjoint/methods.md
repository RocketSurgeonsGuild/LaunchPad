# FluentValidationGrpcJoint Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.Grpc](/LaunchPad/pr-preview/pr-3165/api/rocket.surgery.launchpad.fluentvalidation.grpc.md)
- Type: [FluentValidationGrpcJoint](/LaunchPad/pr-preview/pr-3165/api/rocket.surgery.launchpad.fluentvalidation.grpc/fluentvalidationgrpcjoint.md)
- Kind: `Methods`
- Members: `1`

Validates gRPC request messages with FluentValidation. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

Register the Service part.

```csharp
public class FluentValidationGrpcJoint
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
