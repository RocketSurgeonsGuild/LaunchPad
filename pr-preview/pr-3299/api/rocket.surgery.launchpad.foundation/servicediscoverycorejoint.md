# ServiceDiscoveryCoreJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1252`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3299-merge.Sha.307aa7d68ee152c9493e88e21694727a4cdb86e8/src/Foundation/Joints/ServiceDiscoveryCoreJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that registers service discovery and adds it to every `Http.HttpClient` the application creates.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

public class ServiceDiscoveryCoreJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [ServiceDiscoveryCoreJoint](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.foundation/servicediscoverycorejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.foundation/servicediscoverycorejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
