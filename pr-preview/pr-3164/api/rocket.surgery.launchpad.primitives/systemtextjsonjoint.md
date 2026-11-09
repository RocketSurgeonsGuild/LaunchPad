# SystemTextJsonJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1169`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that configures `Json.JsonSerializerOptions` the LaunchPad way: camel case names and camel case enums. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Joints;

public class SystemTextJsonJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The NodaTime converters are added by the NodaTime package.

## Constructors

- [SystemTextJsonJoint](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/systemtextjsonjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/systemtextjsonjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
