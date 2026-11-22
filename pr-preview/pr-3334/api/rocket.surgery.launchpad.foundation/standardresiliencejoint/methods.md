# StandardResilienceJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.foundation.md)
- Type: [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.foundation/standardresiliencejoint.md)
- Kind: `Methods`
- Members: `1`

A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3334-merge.Sha.1bdca6582f82d4de4d3840709c4a6fa63d4c5f0c/src/Foundation/Joints/StandardResilienceJoint.cs#L16-L19)

Register the Service part.

```csharp
public class StandardResilienceJoint
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
