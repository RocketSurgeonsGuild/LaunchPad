# ExistingValueOptionsFactory<TOptions>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1167`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`
- Inherits: `OptionsFactory<TOptions>`

Implementation of `Options.IOptionsFactory`1`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

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

- [ExistingValueOptionsFactory(TOptions, IEnumerable<IConfigureOptions<TOptions>>, IEnumerable<IPostConfigureOptions<TOptions>>, IEnumerable<IValidateOptions<TOptions>>)](/LaunchPad/pr-preview/pr-3169/api/rocket.surgery.launchpad.primitives/existingvalueoptionsfactory-1/constructors.md#constructor-toptions-ienumerable-iconfigureoptions-toptions-ienumerable-ipostconfigureoptions-toptions-ienumerable-ivalidateoptions-toptions) -- Implementation of `Options.IOptionsFactory`1`.
