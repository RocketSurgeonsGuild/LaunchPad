# BusinessDateRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [BusinessDateRules](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/businessdaterules.md)
- Kind: `Methods`
- Members: `1`

The rules of a business date: a day business is done on, such as a value date or a trading day.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<DateOnly>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/StrongTypes/BusinessDate.cs#L20-L25)

```csharp
public sealed class BusinessDateRules
{
    public static StrongTypeDefinition<DateOnly> Define()
    {
        // ...
    }
}
```
