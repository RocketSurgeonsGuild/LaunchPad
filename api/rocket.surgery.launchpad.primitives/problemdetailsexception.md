# ProblemDetailsException

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.328`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`
- Inherits: `Exception`
- Implements: [IProblemDetailsData](/LaunchPad/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata.md)

NotFoundException.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public abstract class ProblemDetailsException
    : System.Exception,
      Rocket.Surgery.LaunchPad.Primitives.IProblemDetailsData
{
    // ...
}
```

## Properties

- [Instance](/LaunchPad/api/rocket.surgery.launchpad.primitives/problemdetailsexception/properties.md#instance) : `string?` `get; init` -- The instance for the request
- [Link](/LaunchPad/api/rocket.surgery.launchpad.primitives/problemdetailsexception/properties.md#link) : `string?` `get; init` -- Request Type
- [Properties](/LaunchPad/api/rocket.surgery.launchpad.primitives/problemdetailsexception/properties.md#properties) : `IDictionary<string, object?>` `get; init` -- Additional properties
- [Title](/LaunchPad/api/rocket.surgery.launchpad.primitives/problemdetailsexception/properties.md#title) : `string?` `get; init` -- Request title

## See Also

- `Exception`
