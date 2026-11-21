# SpatialJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Spatial](/LaunchPad/pr-preview/pr-3314/api/rocket.surgery.launchpad.spatial.md)
- Type: [SpatialJoint](/LaunchPad/pr-preview/pr-3314/api/rocket.surgery.launchpad.spatial/spatialjoint.md)
- Kind: `Methods`
- Members: `1`

Adds support for spatial types into System.Text.Json

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3314-merge.Sha.a71ddb034db6099f6c3ced498a91cab2ffb1e6c4/src/Spatial/Joints/SpatialJoint.cs#L24-L31)

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
