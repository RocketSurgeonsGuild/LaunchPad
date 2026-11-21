# OpenApiJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [OpenApiJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/openapijoint.md)
- Kind: `Methods`
- Members: `1`

Registers OpenAPI document generation, with the LaunchPad schema and operation transformers. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.dfbe8776933590a8ca1efba66f9af5c4929d4975/src/AspNetCore/Joints/OpenApiJoint.cs#L25-L43)

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
