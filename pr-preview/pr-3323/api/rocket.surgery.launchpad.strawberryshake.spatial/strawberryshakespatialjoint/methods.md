# StrawberryShakeSpatialJoint Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake.Spatial](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.strawberryshake.spatial.md)
- Type: [StrawberryShakeSpatialJoint](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.strawberryshake.spatial/strawberryshakespatialjoint.md)
- Kind: `Methods`
- Members: `1`

Adds the spatial serializers to StrawberryShake

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.848d7df41fb82a23ce0738e64978a35c934d7558/src/StrawberryShake.Spatial/Joints/StrawberryShakeSpatialJoint.cs#L17-L21)

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
