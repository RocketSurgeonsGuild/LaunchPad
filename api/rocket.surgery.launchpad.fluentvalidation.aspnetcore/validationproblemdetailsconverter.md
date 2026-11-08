# ValidationProblemDetailsConverter

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Version: `2.0.1-beta.320`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Validation`
- Target framework: `net10.0`
- Inherits: `JsonConverter<FluentValidationProblemDetails?>`

A RFC 7807 compliant `Serialization.JsonConverter` for [FluentValidationProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

public sealed class ValidationProblemDetailsConverter
    : System.Text.Json.Serialization.JsonConverter<Rocket.Surgery.LaunchPad.AspNetCore.Validation.FluentValidationProblemDetails?>
{
    // ...
}
```

## Constructors

- [ValidationProblemDetailsConverter](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationproblemdetailsconverter/constructors.md#constructor)

## Methods

- [Read(Utf8JsonReader, Type, JsonSerializerOptions)](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationproblemdetailsconverter/methods.md#read-utf8jsonreader-type-jsonserializeroptions) : [FluentValidationProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md) -- Reads and converts the JSON to type [FluentValidationProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md).
- [Write(Utf8JsonWriter, FluentValidationProblemDetails?, JsonSerializerOptions)](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationproblemdetailsconverter/methods.md#write-utf8jsonwriter-fluentvalidationproblemdetails-jsonserializeroptions) -- Writes a specified value as JSON.
