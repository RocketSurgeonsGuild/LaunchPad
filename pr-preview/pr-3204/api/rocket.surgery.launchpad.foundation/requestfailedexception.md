# RequestFailedException

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1196`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Inherits: [ProblemDetailsException](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation/problemdetailsexception.md)

An exception for failed requests, with overloads to take additional details

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public class RequestFailedException
    : Rocket.Surgery.LaunchPad.Foundation.ProblemDetailsException
{
    // ...
}
```

## Remarks

This data can then be used to surface information to the user through something like problem details.

## Constructors

- [RequestFailedException(string)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation/requestfailedexception/constructors.md#constructor-string) -- Initializes a new instance of the [RequestFailedException](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation/requestfailedexception.md) class.
- [RequestFailedException(string, Exception)](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation/requestfailedexception/constructors.md#constructor-string-exception) -- Initializes a new instance of the [RequestFailedException](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation/requestfailedexception.md) class.
- [RequestFailedException](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation/requestfailedexception/constructors.md#constructor) -- Initializes a new instance of the [RequestFailedException](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.foundation/requestfailedexception.md) class.

## See Also

- `Exception`
