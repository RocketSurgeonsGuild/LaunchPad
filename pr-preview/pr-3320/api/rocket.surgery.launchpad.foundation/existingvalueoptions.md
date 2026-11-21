# ExistingValueOptions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1279`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.70bf0a56b876b250c9773da4124343c2e2adfd46/src/Foundation/ExistingValueOptionsFactory.cs)

Helper methods for creating `Foundation.ExistingValueOptionsFactory`1` instances.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public static class ExistingValueOptions
{
    // ...
}
```

## Methods

- [Apply(IServiceProvider, TOptions, string)](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation/existingvalueoptions/methods.md#apply-iserviceprovider-toptions-string) `static` -- Applys all of the `Options.IConfigureOptions`1`, `Options.IPostConfigureOptions`1`, and `Options.IValidateOptions`1` instances to the options instance.
