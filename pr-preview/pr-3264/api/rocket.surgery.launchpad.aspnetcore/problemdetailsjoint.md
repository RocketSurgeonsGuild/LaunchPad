# ProblemDetailsJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3264/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1222`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Registers problem details, filling in their title and type from the client error mapping of `Mvc.ApiBehaviorOptions`. Implements the `Joints.IServiceJoint`

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

- [ProblemDetailsJoint](/LaunchPad/pr-preview/pr-3264/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3264/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
