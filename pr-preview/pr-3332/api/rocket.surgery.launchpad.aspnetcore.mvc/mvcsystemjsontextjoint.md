# MvcSystemJsonTextJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.108cb13ce492322587639698c87355d3b8fcee1b/src/AspNetCore.Mvc/Joints/MvcSystemJsonTextJoint.cs)
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

- [MvcSystemJsonTextJoint](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcsystemjsontextjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcsystemjsontextjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
