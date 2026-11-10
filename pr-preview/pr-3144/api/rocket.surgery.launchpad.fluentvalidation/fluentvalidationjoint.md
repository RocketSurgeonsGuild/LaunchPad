# FluentValidationJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.fluentvalidation.md)
- Version: `2.0.1-pr.1165`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that registers every FluentValidation validator found in the assemblies that depend on FluentValidation and the validators for options. Implements the `Joints.IServiceJoint`

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

The validators are found with the `ISpecularProvider` in the joint context. The generated `Imports` seed the application's Specular provider into the context.

## Constructors

- [FluentValidationJoint](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
