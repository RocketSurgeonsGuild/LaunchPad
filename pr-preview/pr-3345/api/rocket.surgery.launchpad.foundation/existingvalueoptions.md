# ExistingValueOptions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/Foundation/ExistingValueOptionsFactory.cs)

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

- [Apply(IServiceProvider, TOptions, string)](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.foundation/existingvalueoptions/methods.md#apply-iserviceprovider-toptions-string) `static` -- Applys all of the `Options.IConfigureOptions`1`, `Options.IPostConfigureOptions`1`, and `Options.IValidateOptions`1` instances to the options instance.
