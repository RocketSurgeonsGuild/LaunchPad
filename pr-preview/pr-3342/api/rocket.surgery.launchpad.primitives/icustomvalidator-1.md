# ICustomValidator<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3342/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1289`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3342-merge.Sha.fe29130e4542c32d064e5f519feab50b84c0905d/src/Primitives/ICustomValidator.cs)

A rule of a domain primitive written as code, for `[CustomValidator<TValidator>]`. It runs after the domain primitive's other rules.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public interface ICustomValidator<T>
{
    // ...
}
```

## Type Parameters

- `T` -- The value.

## Methods

- [Validate(T)](/LaunchPad/pr-preview/pr-3342/api/rocket.surgery.launchpad.primitives/icustomvalidator-1/methods.md#validate-t) : [PrimitiveError](/LaunchPad/pr-preview/pr-3342/api/rocket.surgery.launchpad.primitives/primitiveerror.md) `static` `abstract` -- Validates the value.
