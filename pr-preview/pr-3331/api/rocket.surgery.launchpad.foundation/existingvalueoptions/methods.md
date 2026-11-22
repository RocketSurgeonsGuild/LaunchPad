# ExistingValueOptions Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.foundation.md)
- Type: [ExistingValueOptions](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.foundation/existingvalueoptions.md)
- Kind: `Methods`
- Members: `1`

Helper methods for creating `Foundation.ExistingValueOptionsFactory`1` instances.

<a id="apply"></a>
<a id="apply-iserviceprovider-toptions-string"></a>

## Apply(IServiceProvider, TOptions, string)

- Name: `Apply(IServiceProvider, TOptions, string)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/Foundation/ExistingValueOptionsFactory.cs#L20-L25)

Applys all of the `Options.IConfigureOptions`1`, `Options.IPostConfigureOptions`1`, and `Options.IValidateOptions`1` instances to the options instance.

```csharp
public static class ExistingValueOptions
{
    public static void Apply<TOptions>(
        IServiceProvider serviceProvider,
        TOptions options,
        string name)
    {
        // ...
    }
}
```

## Parameters

- `serviceProvider` (`IServiceProvider`)
- `options` (`TOptions`)
- `name` (`string`)

## Exceptions

- `OptionsValidationException`
