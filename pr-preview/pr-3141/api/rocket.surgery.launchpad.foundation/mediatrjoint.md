# MediatRJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1152`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that registers MediatR and every handler found in the assemblies that depend on it. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

public class MediatRJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The assemblies are found with the `ISpecularProvider` in the joint context. The generated `Imports` seed the application's Specular provider into the context.

Dovetail resolves the [FoundationOptions](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.foundation/foundationoptions.md) from the joint context when it creates the joint, and they default when none were set.

## Constructors

- [MediatRJoint(FoundationOptions?)](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.foundation/mediatrjoint/constructors.md#constructor-foundationoptions) -- A joint that registers MediatR and every handler found in the assemblies that depend on it. Implements the `Joints.IServiceJoint`

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.foundation/mediatrjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
