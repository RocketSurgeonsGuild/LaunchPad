# BusinessDateRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1279`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.70bf0a56b876b250c9773da4124343c2e2adfd46/src/Primitives/StrongTypes/BusinessDate.cs)
- Implements: `IStrongTypeTemplate<DateOnly>`

The rules of a business date: a day business is done on, such as a value date or a trading day.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class BusinessDateRules
    : SuperStrong.Types.IStrongTypeTemplate<System.DateOnly>
{
    // ...
}
```

## Remarks

- Monday to Friday (ISO 8601 days 1 to 5); Saturdays and Sundays are rejected.
- Public holidays are not checked: they depend on the country, the market and the year. An application that needs them declares its own type with rules that check its calendar.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives/businessdaterules/methods.md#define) : `StrongTypeDefinition<DateOnly>` `static`
