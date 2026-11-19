# ZeroAllocOpenApiJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ZeroAllocOpenApiJoint](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapijoint.md)
- Kind: `Methods`
- Members: `1`

Adds the [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md) to every OpenAPI document, so the ZeroAlloc.Validation rules show in the schemas. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3292-merge.Sha.a3b1ee2389c613a51b34add57c7d3d20cf877d5b/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocOpenApiJoint.cs#L25)

Register the Service part.

```csharp
public class ZeroAllocOpenApiJoint
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
