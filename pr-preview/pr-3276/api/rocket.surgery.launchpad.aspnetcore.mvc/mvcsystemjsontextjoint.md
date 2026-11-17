# MvcSystemJsonTextJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3276/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Version: `2.0.1-pr.1226`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints`
- Target framework: `net11.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Applies the existing System.Text.Json options to MVC's JSON options, as `Joints.SystemJsonTextJoint` does to the HTTP JSON options. Implements the `Joints.IServiceJoint`

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

- [MvcSystemJsonTextJoint](/LaunchPad/pr-preview/pr-3276/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcsystemjsontextjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3276/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcsystemjsontextjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
