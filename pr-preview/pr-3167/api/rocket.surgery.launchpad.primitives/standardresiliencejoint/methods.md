# StandardResilienceJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.primitives.md)
- Type: [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.primitives/standardresiliencejoint.md)
- Kind: `Methods`
- Members: `1`

A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

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
