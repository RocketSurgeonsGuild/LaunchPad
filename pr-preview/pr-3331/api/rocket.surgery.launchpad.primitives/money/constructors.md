# Money Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.primitives.md)
- Type: [Money](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.primitives/money.md)
- Kind: `Constructors`
- Members: `1`

An amount of money in a currency.

<a id="constructor"></a>
<a id="constructor-decimal-currencycode"></a>

## Money(decimal, CurrencyCode)

- Name: `Constructor(decimal, CurrencyCode)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/Primitives/StrongTypes/Money.cs#L24-L29)

Creates an amount of money.

```csharp
public record Money
{
    public Money(
        decimal amount,
        CurrencyCode currency)
    {
        // ...
    }
}
```

## Parameters

- `amount` (`decimal`)
- `currency` ([CurrencyCode](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.primitives/currencycode.md))

## Exceptions

- `ArgumentNullException` -- `currency` is `null`.
