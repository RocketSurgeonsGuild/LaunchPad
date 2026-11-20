# SystemJsonTextJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [SystemJsonTextJoint](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint.md)
- Kind: `Methods`
- Members: `1`

Applies the existing System.Text.Json options to the HTTP JSON options, which Minimal APIs, problem details and OpenAPI use. Rocket.Surgery.LaunchPad.AspNetCore.Mvc applies them to MVC's JSON options too. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3305-merge.Sha.f536a04ca4e28fbdc06da7c1e0f968bbeccd9e9a/src/AspNetCore/Joints/SystemJsonTextJoint.cs#L38-L50)

Registers the specified context.

```csharp
public class SystemJsonTextJoint
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
