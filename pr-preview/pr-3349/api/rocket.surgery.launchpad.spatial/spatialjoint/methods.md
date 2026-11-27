# SpatialJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.spatial.md)
- Type: [SpatialJoint](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.spatial/spatialjoint.md)
- Kind: `Methods`
- Members: `1`

Adds support for spatial types into System.Text.Json

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3349-merge.Sha.70168524ff777a6c4fd1a4bf57611d64095a2c00/src/Spatial/Joints/SpatialJoint.cs#L24-L31)

Register the Service part.

```csharp
public class SpatialJoint
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
