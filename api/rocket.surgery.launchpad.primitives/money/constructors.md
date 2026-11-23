# Money Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [Money](/LaunchPad/api/rocket.surgery.launchpad.primitives/money.md)
- Kind: `Constructors`
- Members: `1`

An amount of money in a currency.

<a id="constructor"></a>
<a id="constructor-decimal-currencycode"></a>

## Money(decimal, CurrencyCode)

- Name: `Constructor(decimal, CurrencyCode)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.54628ef221bd1ad09c6ebc55da16ddcb5027f4c7/src/Primitives/StrongTypes/Money.cs#L24-L29)

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
- `currency` ([CurrencyCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/currencycode.md))

## Exceptions

- `ArgumentNullException` -- `currency` is `null`.
