# FluentValidationProblemDetails

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Version: `2.0.1-pr.1218`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Validation`
- Target framework: `net10.0`
- Inherits: `ValidationProblemDetails`

Problem Details for Fluent Validation

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

public class FluentValidationProblemDetails
    : Microsoft.AspNetCore.Mvc.ValidationProblemDetails
{
    // ...
}
```

## Constructors

- [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails/constructors.md#constructor) -- Construct the Fluent Validation Problem Details
- [FluentValidationProblemDetails(ValidationResult)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails/constructors.md#constructor-validationresult) -- Build Fluent Validation Problem Details from a `Results.ValidationResult`
- [FluentValidationProblemDetails(IEnumerable<ValidationFailure>)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails/constructors.md#constructor-ienumerable-validationfailure) -- Build Fluent Validation Problem Details from a `Generic.IEnumerable`1`

## Properties

- [Rules](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails/properties.md#rules) : `IEnumerable<string>` `get; set` -- The rules run with the validation
- [ValidationErrors](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails/properties.md#validationerrors) : `IDictionary<string, FluentValidationProblemDetail[]>` `get` -- Gets the validation errors associated with this instance of `Foundation.FluentValidationProblemDetail`.
