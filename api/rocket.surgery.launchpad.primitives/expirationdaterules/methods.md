# ExpirationDateRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [ExpirationDateRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/expirationdaterules.md)
- Kind: `Methods`
- Members: `1`

The rules of an expiration date: the last day something (a card, a document, a product) is valid.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<DateOnly>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6c40644535fda9fb8f01149b7373b18e83589412/src/Primitives/StrongTypes/ExpirationDate.cs#L20-L22)

```csharp
public sealed class ExpirationDateRules
{
    public static StrongTypeDefinition<DateOnly> Define()
    {
        // ...
    }
}
```
