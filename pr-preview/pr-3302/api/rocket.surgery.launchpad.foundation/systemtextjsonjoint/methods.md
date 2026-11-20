# SystemTextJsonJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.foundation.md)
- Type: [SystemTextJsonJoint](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.foundation/systemtextjsonjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that configures `Json.JsonSerializerOptions` the LaunchPad way: camel case names, camel case enums, and a `JsonSerializerOptions.TypeInfoResolverChain` built from every registered [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md). Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Foundation/Joints/SystemTextJsonJoint.cs#L35-L57)

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
