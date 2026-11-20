# OpenApiJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-beta.422`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6f19e0829a4004d537d23cc908a809ddcba9219c/src/AspNetCore/Joints/OpenApiJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Registers OpenAPI document generation, with the LaunchPad schema and operation transformers. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class OpenApiJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [OpenApiJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/openapijoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/openapijoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
