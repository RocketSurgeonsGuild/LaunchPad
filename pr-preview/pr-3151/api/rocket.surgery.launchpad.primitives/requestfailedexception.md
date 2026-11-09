# RequestFailedException

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1185`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`
- Inherits: [ProblemDetailsException](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.primitives/problemdetailsexception.md)

An exception for failed requests, with overloads to take additional details

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public class RequestFailedException
    : Rocket.Surgery.LaunchPad.Primitives.ProblemDetailsException
{
    // ...
}
```

## Remarks

This data can then be used to surface information to the user through something like problem details.

## Constructors

- [RequestFailedException(string)](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.primitives/requestfailedexception/constructors.md#constructor-string) -- Initializes a new instance of the [RequestFailedException](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.primitives/requestfailedexception.md) class.
- [RequestFailedException(string, Exception)](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.primitives/requestfailedexception/constructors.md#constructor-string-exception) -- Initializes a new instance of the [RequestFailedException](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.primitives/requestfailedexception.md) class.
- [RequestFailedException](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.primitives/requestfailedexception/constructors.md#constructor) -- Initializes a new instance of the [RequestFailedException](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.primitives/requestfailedexception.md) class.

## See Also

- `Exception`
