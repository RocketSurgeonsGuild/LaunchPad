# Money Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives.md)
- Type: [Money](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/money.md)
- Kind: `Methods`
- Members: `9`

An amount of money in a currency.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Returns: [Money](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/money.md)

```csharp
public record Money
{
    public Money <Clone>$()
    {
        // ...
    }
}
```

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record Money
{
    public override bool Equals(
        object? obj)
    {
        // ...
    }
}
```

## Parameters

- `obj` (`object?`)
  The object to compare with the current object.

## Returns

`bool` -- `true` if the specified object is equal to the current object; otherwise, `false`.

<a id="equals-money"></a>

## Equals(Money?)

- Name: `Equals(Money?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record Money
{
    public bool Equals(
        Money? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Money?](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/money.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="from"></a>
<a id="from-decimal-currencycode"></a>

## From(decimal, CurrencyCode)

- Name: `From(decimal, CurrencyCode)`
- Modifiers: `static`
- Returns: [Money](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/money.md)

Creates an amount of money.

```csharp
public record Money
{
    public static Money From(
        decimal amount,
        CurrencyCode currency)
    {
        // ...
    }
}
```

## Parameters

- `amount` (`decimal`)
- `currency` ([CurrencyCode](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/currencycode.md))

## Exceptions

- `ArgumentNullException` -- `currency` is `null`.

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public record Money
{
    public override int GetHashCode()
    {
        // ...
    }
}
```

## Returns

`int` -- A hash code for the current object.

<a id="op-equality"></a>
<a id="op-equality-money-money"></a>

## op_Equality(Money?, Money?)

- Name: `op_Equality(Money?, Money?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Money
{
    public static bool operator ==(
        Money? left,
        Money? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Money?](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/money.md))
- `right` ([Money?](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/money.md))

<a id="op-inequality"></a>
<a id="op-inequality-money-money"></a>

## op_Inequality(Money?, Money?)

- Name: `op_Inequality(Money?, Money?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Money
{
    public static bool operator !=(
        Money? left,
        Money? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Money?](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/money.md))
- `right` ([Money?](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/money.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record Money
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.

<a id="tryfrom"></a>
<a id="tryfrom-decimal-currencycode-money"></a>

## TryFrom(decimal, CurrencyCode?, Money?)

- Name: `TryFrom(decimal, CurrencyCode?, Money?)`
- Modifiers: `static`
- Returns: `bool`

Creates an amount of money, or returns `false` without a currency.

```csharp
public record Money
{
    public static bool TryFrom(
        decimal amount,
        CurrencyCode? currency,
        out Money? result)
    {
        // ...
    }
}
```

## Parameters

- `amount` (`decimal`)
- `currency` ([CurrencyCode?](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/currencycode.md))
- `result` ([Money?](/LaunchPad/pr-preview/pr-3190/api/rocket.surgery.launchpad.primitives/money.md))
