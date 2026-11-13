# ServiceDiscoveryCoreJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1183`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net10.0`
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

- [ServiceDiscoveryCoreJoint](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation/servicediscoverycorejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation/servicediscoverycorejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
