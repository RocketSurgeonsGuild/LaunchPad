# StandardResilienceJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation.md)
- Type: [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation/standardresiliencejoint.md)
- Kind: `Methods`
- Members: `1`

A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.e1cd1b3273152fe1b84a7e7cf201a85e915d90b0/src/Foundation/Joints/StandardResilienceJoint.cs#L16-L19)

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
