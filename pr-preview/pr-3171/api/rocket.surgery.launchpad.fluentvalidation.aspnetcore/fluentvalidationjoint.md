# FluentValidationJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Version: `2.0.1-pr.1165`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Wires FluentValidation into ASP.NET Core: the client side adapters, the validation exception filter, the problem details converter and the source generated metadata it writes with, and the [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md) as the documented validation response of the restful api controllers. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class FluentValidationJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [FluentValidationJoint](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
