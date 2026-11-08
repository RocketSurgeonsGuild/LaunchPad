# AspNetCoreJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [AspNetCoreJoint](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/aspnetcorejoint.md)
- Kind: `Methods`
- Members: `1`

Registers MVC, the API explorer and the application parts for every assembly that depends on this one. The other ASP.NET Core joints run after this one. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

Registers the MVC services, and adds the application parts found through the Specular provider in the context.

```csharp
public class AspNetCoreJoint
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
  The joint context.
- `services` (`IServiceCollection`)
  The service collection.

## Exceptions

- `KeyNotFoundException` -- No `ISpecularProvider` was set in `context`.
