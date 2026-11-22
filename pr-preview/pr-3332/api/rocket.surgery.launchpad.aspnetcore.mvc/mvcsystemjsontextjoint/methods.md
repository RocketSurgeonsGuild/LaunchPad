# MvcSystemJsonTextJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [MvcSystemJsonTextJoint](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcsystemjsontextjoint.md)
- Kind: `Methods`
- Members: `1`

Applies the existing System.Text.Json options to MVC's JSON options, as `Joints.SystemJsonTextJoint` does to the HTTP JSON options, and chains the metadata of MVC's problem details into both. MVC's JSON options keep ASP.NET Core's reflection resolver: MVC doesn't support native AOT, and its formatters and model binding read types no JSON context declares. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.108cb13ce492322587639698c87355d3b8fcee1b/src/AspNetCore.Mvc/Joints/MvcSystemJsonTextJoint.cs#L27-L31)

Register the Service part.

```csharp
public class MvcSystemJsonTextJoint
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
