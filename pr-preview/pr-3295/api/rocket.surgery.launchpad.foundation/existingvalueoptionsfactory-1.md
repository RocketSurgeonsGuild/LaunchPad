# ExistingValueOptionsFactory<TOptions>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1245`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3295-merge.Sha.8032622be33212edca749d51eeb2f284c76d51b7/src/Foundation/ExistingValueOptionsFactory.cs)
- Inherits: `OptionsFactory<TOptions>`

Implementation of `Options.IOptionsFactory`1`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public class ExistingValueOptionsFactory<TOptions>
    : Microsoft.Extensions.Options.OptionsFactory<TOptions>
    where TOptions : class, new()
{
    // ...
}
```

## Type Parameters

- `TOptions` -- The type of options being requested.

## Remarks

Initializes a new instance with the specified options configurations.

## Constructors

- [ExistingValueOptionsFactory(TOptions, IEnumerable<IConfigureOptions<TOptions>>, IEnumerable<IPostConfigureOptions<TOptions>>, IEnumerable<IValidateOptions<TOptions>>)](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.foundation/existingvalueoptionsfactory-1/constructors.md#constructor-toptions-ienumerable-iconfigureoptions-toptions-ienumerable-ipostconfigureoptions-toptions-ienumerable-ivalidateoptions-toptions) -- Implementation of `Options.IOptionsFactory`1`.
