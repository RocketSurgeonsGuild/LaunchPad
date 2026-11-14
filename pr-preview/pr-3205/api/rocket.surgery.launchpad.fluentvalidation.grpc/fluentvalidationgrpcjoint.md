# FluentValidationGrpcJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.Grpc](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation.grpc.md)
- Version: `2.0.1-pr.1192`
- Namespace: `Rocket.Surgery.LaunchPad.Grpc.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Validates gRPC request messages with FluentValidation. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Grpc.Joints;

public class FluentValidationGrpcJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Runs before the `Joints.GrpcJoint`, so the validation interceptor comes before its exception interceptors.

## Constructors

- [FluentValidationGrpcJoint](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation.grpc/fluentvalidationgrpcjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation.grpc/fluentvalidationgrpcjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
