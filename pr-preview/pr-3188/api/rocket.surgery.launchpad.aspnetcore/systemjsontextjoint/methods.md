# SystemJsonTextJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3188/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [SystemJsonTextJoint](/LaunchPad/pr-preview/pr-3188/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint.md)
- Kind: `Methods`
- Members: `1`

Applies the existing System.Text.Json options to the HTTP JSON options, which Minimal APIs, problem details and OpenAPI use. Rocket.Surgery.LaunchPad.AspNetCore.Mvc applies them to MVC's JSON options too. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

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
