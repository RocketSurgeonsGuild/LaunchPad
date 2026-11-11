# Money

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.340`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IEquatable<Money>`

An amount of money in a currency.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed record Money
    : System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Money>
{
    // ...
}
```

## Remarks

The amount can be any `decimal`, including negative amounts (a refund, a balance owed), and is not rounded to the currency's minor unit, so unit prices with more decimals fit. The currency is required.

Like a strong type, it is valid by construction: [Money.From(decimal, CurrencyCode)](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#from-decimal-currencycode) and the constructor throw without a currency, and [Money.TryFrom(decimal, CurrencyCode?, Money?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#tryfrom-decimal-currencycode-money) returns `false`. It serializes as a JSON object with `amount` and `currency` members, the currency as its ISO 4217 code.

## Constructors

- [Money(decimal, CurrencyCode)](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/constructors.md#constructor-decimal-currencycode) -- Creates an amount of money.

## Properties

- [Amount](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/properties.md#amount) : `decimal` `get` -- The amount, in units of the currency (not minor units: 12.50 US dollars is 12.50).
- [Currency](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/properties.md#currency) : [CurrencyCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/currencycode.md) `get` -- The currency.

## Methods

- [<Clone>$](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#clone) : [Money](/LaunchPad/api/rocket.surgery.launchpad.primitives/money.md)
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Money?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#equals-money) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [From(decimal, CurrencyCode)](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#from-decimal-currencycode) : [Money](/LaunchPad/api/rocket.surgery.launchpad.primitives/money.md) `static` -- Creates an amount of money.
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Money?, Money?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#op-equality-money-money) : `bool` `static`
- [op_Inequality(Money?, Money?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#op-inequality-money-money) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(decimal, CurrencyCode?, Money?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/money/methods.md#tryfrom-decimal-currencycode-money) : `bool` `static` -- Creates an amount of money, or returns `false` without a currency.
