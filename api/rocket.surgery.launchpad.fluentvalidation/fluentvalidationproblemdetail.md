# FluentValidationProblemDetail

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.md)
- Version: `2.0.1-beta.346`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`

A validation error problem

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public class FluentValidationProblemDetail
{
    // ...
}
```

## Constructors

- [FluentValidationProblemDetail](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationproblemdetail/constructors.md#constructor) -- An empty validation error problem, for deserialization.
- [FluentValidationProblemDetail(ValidationFailure)](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationproblemdetail/constructors.md#constructor-validationfailure) -- A validation error problem

## Properties

- [AttemptedValue](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationproblemdetail/properties.md#attemptedvalue) : `object?` `get; set` -- The property value that caused the failure.
- [ErrorCode](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationproblemdetail/properties.md#errorcode) : `string` `get; set` -- Gets or sets the error code.
- [ErrorMessage](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationproblemdetail/properties.md#errormessage) : `string` `get; set` -- The error message
- [PropertyName](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationproblemdetail/properties.md#propertyname) : `string` `get; set` -- The name of the property.
- [Severity](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationproblemdetail/properties.md#severity) : `Severity` `get; set` -- Custom severity level associated with the failure.
