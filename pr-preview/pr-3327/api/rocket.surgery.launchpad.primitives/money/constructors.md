# Money Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.primitives.md)
- Type: [Money](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.primitives/money.md)
- Kind: `Constructors`
- Members: `1`

An amount of money in a currency.

<a id="constructor"></a>
<a id="constructor-decimal-currencycode"></a>

## Money(decimal, CurrencyCode)

- Name: `Constructor(decimal, CurrencyCode)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/Primitives/StrongTypes/Money.cs#L24-L29)

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
- `currency` ([CurrencyCode](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.primitives/currencycode.md))

## Exceptions

- `ArgumentNullException` -- `currency` is `null`.
