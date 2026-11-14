# BusinessDateRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1206`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
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

- [Define](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/businessdaterules/methods.md#define) : `StrongTypeDefinition<DateOnly>` `static`
