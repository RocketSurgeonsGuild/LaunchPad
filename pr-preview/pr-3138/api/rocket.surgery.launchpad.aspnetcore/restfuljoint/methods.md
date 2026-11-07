# RestfulJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [RestfulJoint](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.aspnetcore/restfuljoint.md)
- Kind: `Methods`
- Members: `1`

Registers the controllers, and the application model provider that gives them the restful api conventions. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

Register the Service part.

```csharp
public class RestfulJoint
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
