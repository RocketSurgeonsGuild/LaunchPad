# SystemTextJsonJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.foundation.md)
- Type: [SystemTextJsonJoint](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.foundation/systemtextjsonjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that configures `Json.JsonSerializerOptions` the LaunchPad way: camel case names, camel case enums, and a `JsonSerializerOptions.TypeInfoResolverChain` built from every registered [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md). Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3351-merge.Sha.a0ae38aa18a1b9917789ec429da9c9c80b65406f/src/Foundation/Joints/SystemTextJsonJoint.cs#L35-L57)

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
