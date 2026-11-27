# ServiceDiscoveryCoreJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.458`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6a854476dc50a3b95bf56cf9e7b0d45ee9f0bfa7/src/Foundation/Joints/ServiceDiscoveryCoreJoint.cs)
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

- [ServiceDiscoveryCoreJoint](/LaunchPad/api/rocket.surgery.launchpad.foundation/servicediscoverycorejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.foundation/servicediscoverycorejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
