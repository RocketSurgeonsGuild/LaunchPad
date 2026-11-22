# MvcJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [MvcJoint](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcjoint.md)
- Kind: `Methods`
- Members: `1`

Registers MVC Core, the MVC API explorer and the application parts for every assembly that depends on Rocket.Surgery.LaunchPad.AspNetCore. MVC is not native AOT compatible, which is why this joint lives in Rocket.Surgery.LaunchPad.AspNetCore.Mvc. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/AspNetCore.Mvc/Joints/MvcJoint.cs#L39-L52)

Registers the MVC services, and adds the application parts found through the Specular provider in the context.

```csharp
public class MvcJoint
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
