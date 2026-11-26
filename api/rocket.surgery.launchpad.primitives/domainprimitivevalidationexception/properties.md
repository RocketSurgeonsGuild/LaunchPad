# DomainPrimitiveValidationException Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitiveValidationException](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception.md)
- Kind: `Properties`
- Members: `1`

Thrown when a domain primitive is created from a value its rules reject, unless `LaunchPadPrimitivesExceptionType` names another exception.

<a id="error"></a>

## Error

- Name: `Error`
- Modifiers: `get`
- Returns: [PrimitiveError](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitiveerror.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.0c205f7c0f7c99d017c3755b484180de4bcb850a/src/Primitives/DomainPrimitiveValidationException.cs#L43)

Why the value was rejected.

```csharp
public PrimitiveError Error { get; }
```
