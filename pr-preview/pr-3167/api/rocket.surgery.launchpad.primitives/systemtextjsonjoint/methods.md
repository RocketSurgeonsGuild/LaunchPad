# SystemTextJsonJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.primitives.md)
- Type: [SystemTextJsonJoint](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.primitives/systemtextjsonjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that configures `Json.JsonSerializerOptions` the LaunchPad way: camel case names, camel case enums, and a `JsonSerializerOptions.TypeInfoResolverChain` built from every registered [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.primitives/jsontypeinforesolverregistration.md). Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

Register the Service part.

```csharp
public class SystemTextJsonJoint
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
