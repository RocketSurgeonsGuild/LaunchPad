# MvcJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Version: `2.0.1-pr.1245`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3296-merge.Sha.e2170d7d4b36cc435a264495ab512cf086f53980/src/AspNetCore.Mvc/Joints/MvcJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Registers MVC Core, the MVC API explorer and the application parts for every assembly that depends on Rocket.Surgery.LaunchPad.AspNetCore. MVC is not native AOT compatible, which is why this joint lives in Rocket.Surgery.LaunchPad.AspNetCore.Mvc. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints;

public class MvcJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The application parts are found with the `ISpecularProvider` in the joint context. The generated `Imports` seed the application's Specular provider into the context.

Dovetail resolves the [AspNetCoreMvcOptions](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.aspnetcore.mvc/aspnetcoremvcoptions.md) from the joint context when it creates the joint, and they default when none were set.

## Constructors

- [MvcJoint(AspNetCoreMvcOptions?)](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcjoint/constructors.md#constructor-aspnetcoremvcoptions) -- Registers MVC Core, the MVC API explorer and the application parts for every assembly that depends on Rocket.Surgery.LaunchPad.AspNetCore. MVC is not native AOT compatible, which is why this joint lives in Rocket.Surgery.LaunchPad.AspNetCore.Mvc. Implements the `Joints.IServiceJoint`

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the MVC services, and adds the application parts found through the Specular provider in the context.

## See Also

- `Joints.IServiceJoint`
