# IDomainPrimitive<TSelf, TValue> Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives.md)
- Type: [IDomainPrimitive<TSelf, TValue>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/idomainprimitive-2.md)
- Kind: `Methods`
- Members: `2`

A domain primitive that holds one value.

<a id="create"></a>
<a id="create-tvalue"></a>

## Create(TValue)

- Name: `Create(TValue)`
- Modifiers: `static` `abstract`
- Returns: `TSelf`

Creates the domain primitive from a value, or throws when its rules reject the value.

```csharp
public interface IDomainPrimitive<TSelf, TValue>
{
    public static abstract TSelf IDomainPrimitive<TSelf, Create(TValue value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`TValue`)
  The value.

## Returns

`TSelf` -- The domain primitive.

<a id="trycreate"></a>
<a id="trycreate-tvalue-tself-primitiveerror"></a>

## TryCreate(TValue, TSelf?, PrimitiveError)

- Name: `TryCreate(TValue, TSelf?, PrimitiveError)`
- Modifiers: `static` `abstract`
- Returns: `bool`

Creates the domain primitive from a value, or says why its rules reject the value.

```csharp
public interface IDomainPrimitive<TSelf, TValue>
{
    public static abstract bool IDomainPrimitive<TSelf, TryCreate(TValue value, out TSelf? result, out PrimitiveError validationError)
    {
        // ...
    }
}
```

## Parameters

- `value` (`TValue`)
  The value.
- `result` (`TSelf?`)
  The domain primitive, when the value is accepted.
- `validationError` ([PrimitiveError](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/primitiveerror.md))
  Why the value was rejected, when it was.

## Returns

`bool` -- Whether the value was accepted.
