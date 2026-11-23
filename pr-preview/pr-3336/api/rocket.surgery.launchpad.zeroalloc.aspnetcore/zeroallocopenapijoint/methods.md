# ZeroAllocOpenApiJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ZeroAllocOpenApiJoint](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapijoint.md)
- Kind: `Methods`
- Members: `1`

Adds the [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md) to every OpenAPI document, so the ZeroAlloc.Validation rules show in the schemas. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocOpenApiJoint.cs#L25)

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
