# NodaTimeStrawberryShakeJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Type: [NodaTimeStrawberryShakeJoint](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestrawberryshakejoint.md)
- Kind: `Methods`
- Members: `1`

Adds the NodaTime serializers to StrawberryShake

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/NodaTime.StrawberryShake/Joints/NodaTimeStrawberryShakeJoint.cs#L25-L30)

Register the Service part.

```csharp
public class NodaTimeStrawberryShakeJoint
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
