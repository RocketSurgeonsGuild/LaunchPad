# BusinessDateRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives.md)
- Type: [BusinessDateRules](/LaunchPad/pr-preview/pr-3166/api/rocket.surgery.launchpad.primitives/businessdaterules.md)
- Kind: `Methods`
- Members: `1`

The rules of a business date: a day business is done on, such as a value date or a trading day.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<DateOnly>`

```csharp
public sealed class BusinessDateRules
{
    public static StrongTypeDefinition<DateOnly> Define()
    {
        // ...
    }
}
```
