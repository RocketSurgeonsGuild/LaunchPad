# MvcSystemJsonTextJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [MvcSystemJsonTextJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcsystemjsontextjoint.md)
- Kind: `Methods`
- Members: `1`

Applies the existing System.Text.Json options to MVC's JSON options, as `Joints.SystemJsonTextJoint` does to the HTTP JSON options. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

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
