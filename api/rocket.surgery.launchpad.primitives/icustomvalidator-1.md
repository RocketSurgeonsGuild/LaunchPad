# ICustomValidator<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.453`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.3675b8bfd84082dc0cd600fe8d8fc9c14e97833a/src/Primitives/ICustomValidator.cs)

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

- [Validate(T)](/LaunchPad/api/rocket.surgery.launchpad.primitives/icustomvalidator-1/methods.md#validate-t) : [PrimitiveError](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitiveerror.md) `static` `abstract` -- Validates the value.
