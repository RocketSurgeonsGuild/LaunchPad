# ExpirationDateRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.primitives.md)
- Type: [ExpirationDateRules](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.primitives/expirationdaterules.md)
- Kind: `Methods`
- Members: `1`

The rules of an expiration date: the last day something (a card, a document, a product) is valid.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<DateOnly>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.65b042458886c034a574f04a60cfe2fa08ef3e1a/src/Primitives/StrongTypes/ExpirationDate.cs#L20-L22)

```csharp
public sealed class ExpirationDateRules
{
    public static StrongTypeDefinition<DateOnly> Define()
    {
        // ...
    }
}
```
