# ZeroAllocOpenApiJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ZeroAllocOpenApiJoint](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapijoint.md)
- Kind: `Methods`
- Members: `1`

Adds the [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md) to every OpenAPI document, so the ZeroAlloc.Validation rules show in the schemas. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocOpenApiJoint.cs#L25)

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
