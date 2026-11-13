# SystemTextJsonJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1181`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that configures `Json.JsonSerializerOptions` the LaunchPad way: camel case names, camel case enums, and a `JsonSerializerOptions.TypeInfoResolverChain` built from every registered [JsonTypeInfoResolverRegistration](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md). Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

public class SystemTextJsonJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Register an application's own JSON context (a source generated `Serialization.JsonSerializerContext`) with [LaunchPadJsonServiceCollectionExtensions.AddJsonTypeInfoResolver(IServiceCollection, IJsonTypeInfoResolver, int)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/launchpadjsonservicecollectionextensions/methods.md#addjsontypeinforesolver-iservicecollection-ijsontypeinforesolver-int). It goes in front of LaunchPad's own resolvers. No reflection based resolver is added, so a type no JSON context knows fails the same way in a JIT application as in a native AOT one.

Enums are written as camel case strings by the `Serialization.JsonStringEnumConverter`1` s the package's generator registers with [LaunchPadJsonEnumConverters](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters.md) for the enums the JSON contexts reach; any other enum is written as a number.

The NodaTime converters are added by the NodaTime package.

## Constructors

- [SystemTextJsonJoint](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/systemtextjsonjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.foundation/systemtextjsonjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
