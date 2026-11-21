# StandardResilienceJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.foundation.md)
- Type: [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.foundation/standardresiliencejoint.md)
- Kind: `Methods`
- Members: `1`

A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3326-merge.Sha.03f97a4ece74153c5b06eb64e3df4c70a1e53a62/src/Foundation/Joints/StandardResilienceJoint.cs#L16-L19)

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
