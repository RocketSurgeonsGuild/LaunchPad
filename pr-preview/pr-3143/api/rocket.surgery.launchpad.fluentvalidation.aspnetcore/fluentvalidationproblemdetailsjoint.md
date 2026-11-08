# FluentValidationProblemDetailsJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Version: `2.0.1-pr.1158`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Maps the FluentValidation `ValidationException` to an unprocessable entity, and registers the problem details writers that describe validation failures with [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md). Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class FluentValidationProblemDetailsJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Runs after the `Joints.ProblemDetailsJoint`, so the writers come after the problem details writers it registers, and the status code selector it configures is kept for every other exception.

## Constructors

- [FluentValidationProblemDetailsJoint](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetailsjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetailsjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
- `Joints.ProblemDetailsJoint`
