# StringInValidator<T, TProperty> Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.fluentvalidation.md)
- Type: [StringInValidator<T, TProperty>](/LaunchPad/pr-preview/pr-3167/api/rocket.surgery.launchpad.fluentvalidation/stringinvalidator-2.md)
- Kind: `Methods`
- Members: `1`

A validator that is used to verify the value is in a defined list without creating a specific enum

<a id="isvalid"></a>
<a id="isvalid-validationcontext-t-tproperty"></a>

## IsValid(ValidationContext<T>, TProperty?)

- Name: `IsValid(ValidationContext<T>, TProperty?)`
- Modifiers: `override`
- Returns: `bool`

Validates a specific property value.

```csharp
public class StringInValidator<T, TProperty>
{
    public override bool StringInValidator<T, IsValid(ValidationContext<T> context, TProperty? value)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ValidationContext<T>`)
  The validation context. The parent object can be obtained from here.
- `value` (`TProperty?`)
  The current property value to validate

## Returns

`bool` -- True if valid, otherwise false.
