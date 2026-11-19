# BusinessDateRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3290/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1245`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3290-merge.Sha.b94df74c1508b539ddbbc37c7a0b588136261189/src/Primitives/StrongTypes/BusinessDate.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3290/api/rocket.surgery.launchpad.primitives/businessdaterules/methods.md#define) : `StrongTypeDefinition<DateOnly>` `static`
