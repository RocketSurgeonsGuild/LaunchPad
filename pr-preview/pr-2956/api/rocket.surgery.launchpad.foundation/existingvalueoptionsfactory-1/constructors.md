# ExistingValueOptionsFactory<TOptions> Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.foundation.md)
- Type: [ExistingValueOptionsFactory<TOptions>](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.foundation/existingvalueoptionsfactory-1.md)
- Kind: `Constructors`
- Members: `1`

Implementation of `Options.IOptionsFactory`1`.

<a id="constructor"></a>
<a id="constructor-toptions-ienumerable-iconfigureoptions-toptions-ienumerable-ipostconfigureoptions-toptions-ienumerable-ivalidateoptions-toptions"></a>

## ExistingValueOptionsFactory(TOptions, IEnumerable<IConfigureOptions<TOptions>>, IEnumerable<IPostConfigureOptions<TOptions>>, IEnumerable<IValidateOptions<TOptions>>)

- Name: `Constructor(TOptions, IEnumerable<IConfigureOptions<TOptions>>, IEnumerable<IPostConfigureOptions<TOptions>>, IEnumerable<IValidateOptions<TOptions>>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2956-merge.Sha.e1593c4f42603ac4a494d6f792fa4b45d268d971/src/Foundation/ExistingValueOptionsFactory.cs#L45-L51)

Implementation of `Options.IOptionsFactory`1`.

```csharp
public class ExistingValueOptionsFactory<TOptions>
{
    public ExistingValueOptionsFactory(
        TOptions instance,
        IEnumerable<IConfigureOptions<TOptions>> setups,
        IEnumerable<IPostConfigureOptions<TOptions>> postConfigures,
        IEnumerable<IValidateOptions<TOptions>> validations)
    {
        // ...
    }
}
```

## Parameters

- `instance` (`TOptions`)
  The instance to configure
- `setups` (`IEnumerable<IConfigureOptions<TOptions>>`)
  The configuration actions to run.
- `postConfigures` (`IEnumerable<IPostConfigureOptions<TOptions>>`)
  The initialization actions to run.
- `validations` (`IEnumerable<IValidateOptions<TOptions>>`)
  The validations to run.

## Remarks

Initializes a new instance with the specified options configurations.
