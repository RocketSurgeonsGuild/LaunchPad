# StandardResilienceJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.f470c0fad9a519f7357ef1c1f00f2210962f5308/src/Foundation/Joints/StandardResilienceJoint.cs)
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

- [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.foundation/standardresiliencejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.foundation/standardresiliencejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
