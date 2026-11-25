# ICustomValidator<T> Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [ICustomValidator<T>](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/icustomvalidator-1.md)
- Kind: `Methods`
- Members: `1`

A rule of a domain primitive written as code, for `[CustomValidator<TValidator>]`. It runs after the domain primitive's other rules.

<a id="validate"></a>
<a id="validate-t"></a>

## Validate(T)

- Name: `Validate(T)`
- Modifiers: `static` `abstract`
- Returns: [PrimitiveError](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitiveerror.md)

Validates the value.

```csharp
public interface ICustomValidator<T>
{
    public static abstract PrimitiveError Validate(
        T value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`T`)
  The value.

## Returns

[PrimitiveError](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitiveerror.md) -- [PrimitiveError.None](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitiveerror/properties.md#none) when the value is accepted, otherwise why it is not.
