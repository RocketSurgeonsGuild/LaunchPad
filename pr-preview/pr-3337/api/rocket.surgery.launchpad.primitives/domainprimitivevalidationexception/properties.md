# DomainPrimitiveValidationException Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitiveValidationException](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception.md)
- Kind: `Properties`
- Members: `1`

Thrown when a domain primitive is created from a value its rules reject, unless `LaunchPadPrimitivesExceptionType` names another exception.

<a id="error"></a>

## Error

- Name: `Error`
- Modifiers: `get`
- Returns: [PrimitiveError](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitiveerror.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.7307fcdde362b2224f6c0071b6dfb1eaf21e5cff/src/Primitives/DomainPrimitiveValidationException.cs#L43)

Why the value was rejected.

```csharp
public PrimitiveError Error { get; }
```
