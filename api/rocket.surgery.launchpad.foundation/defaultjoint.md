# DefaultJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.352`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net10.0`
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

- [DefaultJoint](/LaunchPad/api/rocket.surgery.launchpad.foundation/defaultjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.foundation/defaultjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
