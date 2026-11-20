# ExpirationDateRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2956-merge.Sha.e1593c4f42603ac4a494d6f792fa4b45d268d971/src/Primitives/StrongTypes/ExpirationDate.cs)
- Implements: `IStrongTypeTemplate<DateOnly>`

The rules of an expiration date: the last day something (a card, a document, a product) is valid.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class ExpirationDateRules
    : SuperStrong.Types.IStrongTypeTemplate<System.DateOnly>
{
    // ...
}
```

## Remarks

- Any date except `DateOnly.MinValue`, the default of an unset `DateOnly`. A past date is accepted: an expired card or document still has an expiration date, and must still be read back from storage. Use [ExpirationDate.IsExpiredOn(DateOnly)](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#isexpiredon-dateonly) to check it against a day.

## Methods

- [Define](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.primitives/expirationdaterules/methods.md#define) : `StrongTypeDefinition<DateOnly>` `static`
