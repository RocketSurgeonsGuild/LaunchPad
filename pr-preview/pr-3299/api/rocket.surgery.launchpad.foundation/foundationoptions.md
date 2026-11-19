# FoundationOptions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1252`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3299-merge.Sha.307aa7d68ee152c9493e88e21694727a4cdb86e8/src/Foundation/FoundationOptions.cs)

Common foundation options

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public class FoundationOptions
{
    // ...
}
```

## Constructors

- [FoundationOptions](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.foundation/foundationoptions/constructors.md#constructor)

## Properties

- [RegisterValidationOptionsAsHealthChecks](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.foundation/foundationoptions/properties.md#registervalidationoptionsashealthchecks) : `bool?` `get; set` -- Validation options are registered as health checks instead of throwing and stopping application startup
- [ValidatorLifetime](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.foundation/foundationoptions/properties.md#validatorlifetime) : `ServiceLifetime` `get; set` -- The Validator lifetime
