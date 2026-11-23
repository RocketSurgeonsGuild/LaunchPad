# StandardResilienceJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1285`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.28f891b4cf6958b8ac5addf5a93485615fecf518/src/Foundation/Joints/StandardResilienceJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

public class StandardResilienceJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation/standardresiliencejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation/standardresiliencejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
