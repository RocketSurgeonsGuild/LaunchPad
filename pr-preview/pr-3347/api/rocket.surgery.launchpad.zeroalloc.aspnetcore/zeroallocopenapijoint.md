# ZeroAllocOpenApiJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1300`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocOpenApiJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Adds the [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md) to every OpenAPI document, so the ZeroAlloc.Validation rules show in the schemas. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class ZeroAllocOpenApiJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Runs after the `Joints.OpenApiJoint`, which registers the OpenAPI document generation.

## Constructors

- [ZeroAllocOpenApiJoint](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapijoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapijoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
- `Joints.OpenApiJoint`
