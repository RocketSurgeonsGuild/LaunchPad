# NodaTimeSystemTextJsonJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeSystemTextJsonJoint](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that configures `Json.JsonSerializerOptions` with the NodaTime converters, using the `IDateTimeZoneProvider` that is registered. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3334-merge.Sha.1bdca6582f82d4de4d3840709c4a6fa63d4c5f0c/src/NodaTime/Joints/NodaTimeSystemTextJsonJoint.cs#L27-L37)

Register the Service part.

```csharp
public class NodaTimeSystemTextJsonJoint
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
