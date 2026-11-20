# Money Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.primitives.md)
- Type: [Money](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.primitives/money.md)
- Kind: `Properties`
- Members: `2`

An amount of money in a currency.

<a id="amount"></a>

## Amount

- Name: `Amount`
- Modifiers: `get`
- Returns: `decimal`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3305-merge.Sha.f536a04ca4e28fbdc06da7c1e0f968bbeccd9e9a/src/Primitives/StrongTypes/Money.cs)

The amount, in units of the currency (not minor units: 12.50 US dollars is 12.50).

```csharp
public decimal Amount { get; }
```

<a id="currency"></a>

## Currency

- Name: `Currency`
- Modifiers: `get`
- Returns: [CurrencyCode](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.primitives/currencycode.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3305-merge.Sha.f536a04ca4e28fbdc06da7c1e0f968bbeccd9e9a/src/Primitives/StrongTypes/Money.cs)

The currency.

```csharp
public CurrencyCode Currency { get; }
```
