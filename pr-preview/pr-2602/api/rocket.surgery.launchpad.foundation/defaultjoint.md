# DefaultJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1259`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/Foundation/Joints/DefaultJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

The joint every LaunchPad application starts from. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

public class DefaultJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Adds the options and logging services, then registers every class marked with Specular's service registration attributes using the `ISpecularProvider` found in the joint context.

The provider is not created here: the generated `Imports` seed the application's Specular provider into the context.

## Constructors

- [DefaultJoint](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation/defaultjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation/defaultjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
