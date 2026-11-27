# ProblemDetailsJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-beta.458`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6a854476dc50a3b95bf56cf9e7b0d45ee9f0bfa7/src/AspNetCore/Joints/ProblemDetailsJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Registers problem details. Rocket.Surgery.LaunchPad.AspNetCore.Mvc also fills in their title and type from MVC's client error mapping. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class ProblemDetailsJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [ProblemDetailsJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
