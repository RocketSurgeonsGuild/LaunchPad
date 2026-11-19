# VinRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.primitives.md)
- Type: [VinRules](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.primitives/vinrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a VIN, a vehicle identification number (ISO 3779), such as `1M8GDM9AXKP042788`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.3325c774c1aaaf8fd0e7d3543b322e84255d322c/src/Primitives/StrongTypes/Vin.cs#L28-L31)

```csharp
public sealed class VinRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
