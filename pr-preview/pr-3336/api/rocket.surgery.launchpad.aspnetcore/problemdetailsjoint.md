# ProblemDetailsJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/AspNetCore/Joints/ProblemDetailsJoint.cs)
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

- [ProblemDetailsJoint](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
