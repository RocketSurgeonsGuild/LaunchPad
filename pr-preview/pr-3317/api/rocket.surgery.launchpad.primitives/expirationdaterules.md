# ExpirationDateRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1270`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3317-merge.Sha.1621484e9867a9384634a5b693a135763b433b4a/src/Primitives/StrongTypes/ExpirationDate.cs)
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

- Any date except `DateOnly.MinValue`, the default of an unset `DateOnly`. A past date is accepted: an expired card or document still has an expiration date, and must still be read back from storage. Use [ExpirationDate.IsExpiredOn(DateOnly)](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.primitives/expirationdate/methods.md#isexpiredon-dateonly) to check it against a day.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.primitives/expirationdaterules/methods.md#define) : `StrongTypeDefinition<DateOnly>` `static`
