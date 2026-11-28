# DomainPrimitiveValidationException

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/DomainPrimitiveValidationException.cs)
- Inherits: `ArgumentException`

Thrown when a domain primitive is created from a value its rules reject, unless `LaunchPadPrimitivesExceptionType` names another exception.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public sealed class DomainPrimitiveValidationException
    : System.ArgumentException
{
    // ...
}
```

## Constructors

- [DomainPrimitiveValidationException(PrimitiveError)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror) -- Creates the exception.
- [DomainPrimitiveValidationException(PrimitiveError, string)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror-string) -- Creates the exception.
- [DomainPrimitiveValidationException(PrimitiveError, Exception?)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror-exception) -- Creates the exception.
- [DomainPrimitiveValidationException(PrimitiveError, Exception?, string)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror-exception-string) -- Creates the exception.

## Properties

- [Error](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/properties.md#error) : [PrimitiveError](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror.md) `get` -- Why the value was rejected.
