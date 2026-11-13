# FluentValidationOpenApiJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Adds the experimental FluentValidation schema transformer to the OpenAPI document, so the validation rules show in the schemas. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class FluentValidationOpenApiJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Runs after the `Joints.OpenApiJoint`, which registers the OpenAPI document generation.

## Constructors

- [FluentValidationOpenApiJoint](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationopenapijoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationopenapijoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
- `Joints.OpenApiJoint`
