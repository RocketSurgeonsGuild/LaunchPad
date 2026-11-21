# VinRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives.md)
- Type: [VinRules](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/vinrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a VIN, a vehicle identification number (ISO 3779), such as `1M8GDM9AXKP042788`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/Primitives/StrongTypes/Vin.cs#L28-L31)

```csharp
public sealed class VinRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
