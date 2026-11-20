# MvcSystemJsonTextJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Version: `2.0.1-beta.422`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6f19e0829a4004d537d23cc908a809ddcba9219c/src/AspNetCore.Mvc/Joints/MvcSystemJsonTextJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Applies the existing System.Text.Json options to MVC's JSON options, as `Joints.SystemJsonTextJoint` does to the HTTP JSON options, and chains the metadata of MVC's problem details into both. MVC's JSON options keep ASP.NET Core's reflection resolver: MVC doesn't support native AOT, and its formatters and model binding read types no JSON context declares. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints;

public class MvcSystemJsonTextJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [MvcSystemJsonTextJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcsystemjsontextjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcsystemjsontextjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
