# OpenApiJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [OpenApiJoint](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.aspnetcore/openapijoint.md)
- Kind: `Methods`
- Members: `1`

Registers OpenAPI document generation, with the LaunchPad schema and operation transformers. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/AspNetCore/Joints/OpenApiJoint.cs#L25-L43)

Registers the specified context.

```csharp
public class OpenApiJoint
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
  The context.
- `services` (`IServiceCollection`)
  The service collection.
