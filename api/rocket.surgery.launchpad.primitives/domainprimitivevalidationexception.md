# DomainPrimitiveValidationException

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.458`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6a854476dc50a3b95bf56cf9e7b0d45ee9f0bfa7/src/Primitives/DomainPrimitiveValidationException.cs)
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

- [DomainPrimitiveValidationException(PrimitiveError)](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror) -- Creates the exception.
- [DomainPrimitiveValidationException(PrimitiveError, string)](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror-string) -- Creates the exception.
- [DomainPrimitiveValidationException(PrimitiveError, Exception?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror-exception) -- Creates the exception.
- [DomainPrimitiveValidationException(PrimitiveError, Exception?, string)](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/constructors.md#constructor-primitiveerror-exception-string) -- Creates the exception.

## Properties

- [Error](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception/properties.md#error) : [PrimitiveError](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitiveerror.md) `get` -- Why the value was rejected.
