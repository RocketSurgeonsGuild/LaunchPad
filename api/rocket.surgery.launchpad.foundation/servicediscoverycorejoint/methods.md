# ServiceDiscoveryCoreJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [ServiceDiscoveryCoreJoint](/LaunchPad/api/rocket.surgery.launchpad.foundation/servicediscoverycorejoint.md)
- Kind: `Methods`
- Members: `1`

A joint that registers service discovery and adds it to every `Http.HttpClient` the application creates.

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be4f65663b97e5bb7c221e0ce4f3da85894aeb85/src/Foundation/Joints/ServiceDiscoveryCoreJoint.cs#L16-L20)

Register the Service part.

```csharp
public class ServiceDiscoveryCoreJoint
{
    public void Register(
        IDovetailContext context,
        IServiceCollection services)
    {
        // ...
    }
}
```

## Parameters

- `context` (`IDovetailContext`)
- `services` (`IServiceCollection`)
