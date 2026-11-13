# ProblemDetailsException

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Inherits: `Exception`
- Implements: [IProblemDetailsData](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata.md)

NotFoundException.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public abstract class ProblemDetailsException
    : System.Exception,
      Rocket.Surgery.LaunchPad.Foundation.IProblemDetailsData
{
    // ...
}
```

## Properties

- [Instance](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.foundation/problemdetailsexception/properties.md#instance) : `string?` `get; init` -- The instance for the request
- [Link](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.foundation/problemdetailsexception/properties.md#link) : `string?` `get; init` -- Request Type
- [Properties](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.foundation/problemdetailsexception/properties.md#properties) : `IDictionary<string, object?>` `get; init` -- Additional properties
- [Title](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.foundation/problemdetailsexception/properties.md#title) : `string?` `get; init` -- Request title

## See Also

- `Exception`
