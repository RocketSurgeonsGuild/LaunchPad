# StrawberryShakeSpatialJoint Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3304/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Type: [StrawberryShakeSpatialJoint](/LaunchPad/pr-preview/pr-3304/api/rocket.surgery.launchpad.strawberryshake.spatial/strawberryshakespatialjoint.md)
- Kind: `Methods`
- Members: `1`

Adds the spatial serializers to StrawberryShake

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3304-merge.Sha.ea4753f3bbfef2a34e4933806cae0d39e6651456/src/StrawberryShake.Spatial/Joints/StrawberryShakeSpatialJoint.cs#L17-L21)

Register the Service part.

```csharp
public class StrawberryShakeSpatialJoint
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
