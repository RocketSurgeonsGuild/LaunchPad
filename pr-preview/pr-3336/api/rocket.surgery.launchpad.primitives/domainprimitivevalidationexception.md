# DomainPrimitiveValidationException

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/DomainPrimitiveValidationException.cs)
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

- [DomainPrimitiveValidationException(PrimitiveError)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror) -- Creates the exception.
- [DomainPrimitiveValidationException(PrimitiveError, string)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror-string) -- Creates the exception.
- [DomainPrimitiveValidationException(PrimitiveError, Exception?)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror-exception) -- Creates the exception.
- [DomainPrimitiveValidationException(PrimitiveError, Exception?, string)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror-exception-string) -- Creates the exception.

## Properties

- [Error](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/properties.md#error) : [PrimitiveError](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitiveerror.md) `get` -- Why the value was rejected.
