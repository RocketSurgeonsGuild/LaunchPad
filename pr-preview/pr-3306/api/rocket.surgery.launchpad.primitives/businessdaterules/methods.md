# BusinessDateRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3306/api/rocket.surgery.launchpad.primitives.md)
- Type: [BusinessDateRules](/LaunchPad/pr-preview/pr-3306/api/rocket.surgery.launchpad.primitives/businessdaterules.md)
- Kind: `Methods`
- Members: `1`

The rules of a business date: a day business is done on, such as a value date or a trading day.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<DateOnly>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3306-merge.Sha.7731056fc28c1fb31fe802623d663df67f58b451/src/Primitives/StrongTypes/BusinessDate.cs#L20-L25)

```csharp
public sealed class BusinessDateRules
{
    public static StrongTypeDefinition<DateOnly> Define()
    {
        // ...
    }
}
```
