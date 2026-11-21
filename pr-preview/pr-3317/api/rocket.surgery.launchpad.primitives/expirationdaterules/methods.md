# ExpirationDateRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.primitives.md)
- Type: [ExpirationDateRules](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.primitives/expirationdaterules.md)
- Kind: `Methods`
- Members: `1`

The rules of an expiration date: the last day something (a card, a document, a product) is valid.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<DateOnly>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3317-merge.Sha.125c9fda2ca8a5e1aa1ca7ddbbdcadbcdb967166/src/Primitives/StrongTypes/ExpirationDate.cs#L20-L22)

```csharp
public sealed class ExpirationDateRules
{
    public static StrongTypeDefinition<DateOnly> Define()
    {
        // ...
    }
}
```
