# SpatialJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.spatial.md)
- Type: [SpatialJoint](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.spatial/spatialjoint.md)
- Kind: `Methods`
- Members: `1`

Adds support for spatial types into System.Text.Json

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.45fabcb1eb2968ce041767000387c82329263a76/src/Spatial/Joints/SpatialJoint.cs#L24-L31)

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
