# FluentValidationJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.fluentvalidation.md)
- Version: `2.0.1-pr.1163`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that registers every FluentValidation validator found in the assemblies that depend on FluentValidation, the validators for options, and the MediatR validation pipeline behaviors. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

public class FluentValidationJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Runs after the `Joints.MediatRJoint`, so the validation behaviors are added to the pipeline MediatR registers.

The validators are found with the `ISpecularProvider` in the joint context. The generated `Imports` seed the application's Specular provider into the context.

Dovetail resolves the `Foundation.FoundationOptions` from the joint context when it creates the joint, and they default when none were set.

## Constructors

- [FluentValidationJoint(FoundationOptions?)](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationjoint/constructors.md#constructor-foundationoptions) -- A joint that registers every FluentValidation validator found in the assemblies that depend on FluentValidation, the validators for options, and the MediatR validation pipeline behaviors. Implements the `Joints.IServiceJoint`

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3158/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
- `Joints.MediatRJoint`
