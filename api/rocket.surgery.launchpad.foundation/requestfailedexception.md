# RequestFailedException

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.332`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Inherits: [ProblemDetailsException](/LaunchPad/api/rocket.surgery.launchpad.foundation/problemdetailsexception.md)

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

- [RequestFailedException(string)](/LaunchPad/api/rocket.surgery.launchpad.foundation/requestfailedexception/constructors.md#constructor-string) -- Initializes a new instance of the [RequestFailedException](/LaunchPad/api/rocket.surgery.launchpad.foundation/requestfailedexception.md) class.
- [RequestFailedException(string, Exception)](/LaunchPad/api/rocket.surgery.launchpad.foundation/requestfailedexception/constructors.md#constructor-string-exception) -- Initializes a new instance of the [RequestFailedException](/LaunchPad/api/rocket.surgery.launchpad.foundation/requestfailedexception.md) class.
- [RequestFailedException](/LaunchPad/api/rocket.surgery.launchpad.foundation/requestfailedexception/constructors.md#constructor) -- Initializes a new instance of the [RequestFailedException](/LaunchPad/api/rocket.surgery.launchpad.foundation/requestfailedexception.md) class.

## See Also

- `Exception`
