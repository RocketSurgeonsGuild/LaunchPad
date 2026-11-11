# Money Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives.md)
- Type: [Money](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/money.md)
- Kind: `Constructors`
- Members: `1`

An amount of money in a currency.

<a id="constructor"></a>
<a id="constructor-decimal-currencycode"></a>

## Money(decimal, CurrencyCode)

- Name: `Constructor(decimal, CurrencyCode)`

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
- `currency` ([CurrencyCode](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.primitives/currencycode.md))

## Exceptions

- `ArgumentNullException` -- `currency` is `null`.
