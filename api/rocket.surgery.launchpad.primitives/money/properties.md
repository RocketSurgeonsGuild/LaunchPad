# Money Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [Money](/LaunchPad/api/rocket.surgery.launchpad.primitives/money.md)
- Kind: `Properties`
- Members: `2`

An amount of money in a currency.

<a id="amount"></a>

## Amount

- Name: `Amount`
- Modifiers: `get`
- Returns: `decimal`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.d5743b8445e2048c34f1a4b585f3eb4d862008a8/src/Primitives/StrongTypes/Money.cs)

The amount, in units of the currency (not minor units: 12.50 US dollars is 12.50).

```csharp
public decimal Amount { get; }
```

<a id="currency"></a>

## Currency

- Name: `Currency`
- Modifiers: `get`
- Returns: [CurrencyCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/currencycode.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.d5743b8445e2048c34f1a4b585f3eb4d862008a8/src/Primitives/StrongTypes/Money.cs)

The currency.

```csharp
public CurrencyCode Currency { get; }
```
