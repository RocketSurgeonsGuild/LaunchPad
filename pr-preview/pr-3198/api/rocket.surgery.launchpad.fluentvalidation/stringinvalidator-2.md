# StringInValidator<T, TProperty>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation.md)
- Version: `2.0.1-pr.1180`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Validation`
- Target framework: `net10.0`
- Inherits: `PropertyValidator<T, TProperty?>`
- Implements: `IStringInValidator`

A validator that is used to verify the value is in a defined list without creating a specific enum

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

public class StringInValidator<T, TProperty>
    : FluentValidation.Validators.PropertyValidator<T, TProperty?>,
      Rocket.Surgery.LaunchPad.Foundation.Validation.IStringInValidator
    where TProperty : notnull
{
    // ...
}
```

## Constructors

- [StringInValidator(string[], bool)](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation/stringinvalidator-2/constructors.md#constructor-string-bool) -- A validator that is used to verify the value is in a defined list without creating a specific enum

## Properties

- [Name](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation/stringinvalidator-2/properties.md#name) : `string` `get` -- The name of the validator. This is usually the type name without any generic parameters. This is used as the default Error Code for the validator.
- [Values](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation/stringinvalidator-2/properties.md#values) : `IEnumerable<string>` `get` -- The values that being enforced.

## Methods

- [IsValid(ValidationContext<T>, TProperty?)](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation/stringinvalidator-2/methods.md#isvalid-validationcontext-t-tproperty) : `bool` -- Validates a specific property value.
