# ZeroAllocOpenApiJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.022b9cee91d8927e916725e8a1c1717558619950/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocOpenApiJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Adds the [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md) to every OpenAPI document, so the ZeroAlloc.Validation rules show in the schemas. Implements the `Joints.IServiceJoint`

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

- [ZeroAllocOpenApiJoint](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapijoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapijoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
- `Joints.OpenApiJoint`
