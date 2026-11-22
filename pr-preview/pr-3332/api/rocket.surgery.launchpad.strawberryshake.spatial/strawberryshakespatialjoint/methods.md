# StrawberryShakeSpatialJoint Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Type: [StrawberryShakeSpatialJoint](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.strawberryshake.spatial/strawberryshakespatialjoint.md)
- Kind: `Methods`
- Members: `1`

Adds the spatial serializers to StrawberryShake

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.fb40f4ae51769f679795c3f90ed1c13a62505cf5/src/StrawberryShake.Spatial/Joints/StrawberryShakeSpatialJoint.cs#L17-L21)

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
