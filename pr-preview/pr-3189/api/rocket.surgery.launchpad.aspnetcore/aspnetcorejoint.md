# AspNetCoreJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1177`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Registers MVC, the API explorer and the application parts for every assembly that depends on this one. The other ASP.NET Core joints run after this one. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class AspNetCoreJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The application parts are found with the `ISpecularProvider` in the joint context. The generated `Imports` seed the application's Specular provider into the context.

Dovetail resolves the [AspNetCoreOptions](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.aspnetcore/aspnetcoreoptions.md) from the joint context when it creates the joint, and they default when none were set.

## Constructors

- [AspNetCoreJoint(AspNetCoreOptions?)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.aspnetcore/aspnetcorejoint/constructors.md#constructor-aspnetcoreoptions) -- Registers MVC, the API explorer and the application parts for every assembly that depends on this one. The other ASP.NET Core joints run after this one. Implements the `Joints.IServiceJoint`

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.aspnetcore/aspnetcorejoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the MVC services, and adds the application parts found through the Specular provider in the context.

## See Also

- `Joints.IServiceJoint`
