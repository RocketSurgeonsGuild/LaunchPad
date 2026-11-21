# SpatialJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.spatial.md)
- Type: [SpatialJoint](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.spatial/spatialjoint.md)
- Kind: `Methods`
- Members: `1`

Adds support for spatial types into System.Text.Json

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3310-merge.Sha.8860010d83b5d02194514237744998b27c36d06d/src/Spatial/Joints/SpatialJoint.cs#L24-L31)

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
