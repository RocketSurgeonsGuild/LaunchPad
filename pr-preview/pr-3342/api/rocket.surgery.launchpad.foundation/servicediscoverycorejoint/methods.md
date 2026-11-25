# ServiceDiscoveryCoreJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3342/api/rocket.surgery.launchpad.foundation.md)
- Type: [ServiceDiscoveryCoreJoint](/LaunchPad/pr-preview/pr-3342/api/rocket.surgery.launchpad.foundation/servicediscoverycorejoint.md)
- Kind: `Methods`
- Members: `1`

A joint that registers service discovery and adds it to every `Http.HttpClient` the application creates.

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3342-merge.Sha.fe29130e4542c32d064e5f519feab50b84c0905d/src/Foundation/Joints/ServiceDiscoveryCoreJoint.cs#L16-L20)

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
