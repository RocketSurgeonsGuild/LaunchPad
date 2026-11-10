# ServiceDiscoveryCoreJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives.md)
- Type: [ServiceDiscoveryCoreJoint](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/servicediscoverycorejoint.md)
- Kind: `Methods`
- Members: `1`

A joint that registers service discovery and adds it to every `Http.HttpClient` the application creates.

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

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
