# NodaTimeStrawberryShakeJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Type: [NodaTimeStrawberryShakeJoint](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestrawberryshakejoint.md)
- Kind: `Methods`
- Members: `1`

Adds the NodaTime serializers to StrawberryShake

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3295-merge.Sha.8032622be33212edca749d51eeb2f284c76d51b7/src/NodaTime.StrawberryShake/Joints/NodaTimeStrawberryShakeJoint.cs#L25-L30)

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
