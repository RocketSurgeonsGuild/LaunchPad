# OpenApiJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [OpenApiJoint](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore/openapijoint.md)
- Kind: `Methods`
- Members: `1`

Registers OpenAPI document generation, with the LaunchPad schema and operation transformers. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/AspNetCore/Joints/OpenApiJoint.cs#L25-L43)

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
