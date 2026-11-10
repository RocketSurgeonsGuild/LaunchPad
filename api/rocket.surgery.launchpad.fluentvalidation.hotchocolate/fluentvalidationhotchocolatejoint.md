# FluentValidationHotChocolateJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.hotchocolate.md)
- Version: `2.0.1-beta.331`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Validates GraphQL arguments with FluentValidation through FairyBread.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

public class FluentValidationHotChocolateJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Adds FairyBread to the GraphQL builder the other GraphQL joints share, finds the validators in the service collection, and runs before the `Joints.HotChocolateJoint`.

## Constructors

- [FluentValidationHotChocolateJoint](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.hotchocolate/fluentvalidationhotchocolatejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.hotchocolate/fluentvalidationhotchocolatejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
