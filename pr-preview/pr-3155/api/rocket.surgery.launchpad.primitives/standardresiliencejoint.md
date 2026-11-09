# StandardResilienceJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3155/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1168`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Joints;

public class StandardResilienceJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3155/api/rocket.surgery.launchpad.primitives/standardresiliencejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3155/api/rocket.surgery.launchpad.primitives/standardresiliencejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
