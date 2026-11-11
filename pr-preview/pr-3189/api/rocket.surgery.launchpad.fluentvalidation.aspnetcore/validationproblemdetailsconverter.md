# ValidationProblemDetailsConverter

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Version: `2.0.1-pr.1177`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Validation`
- Target framework: `net10.0`
- Inherits: `JsonConverter<FluentValidationProblemDetails?>`

A RFC 7807 compliant `Serialization.JsonConverter` for [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

public sealed class ValidationProblemDetailsConverter
    : System.Text.Json.Serialization.JsonConverter<Rocket.Surgery.LaunchPad.AspNetCore.Validation.FluentValidationProblemDetails?>
{
    // ...
}
```

## Remarks

The problem details are written with the metadata the options' resolver chain has for them, which the `FluentValidationJoint` adds, so the converter does not need reflection. Options without it fall back to this package's own source generated metadata.

## Constructors

- [ValidationProblemDetailsConverter](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationproblemdetailsconverter/constructors.md#constructor)

## Methods

- [Read(Utf8JsonReader, Type, JsonSerializerOptions)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationproblemdetailsconverter/methods.md#read-utf8jsonreader-type-jsonserializeroptions) : [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md) -- Reads and converts the JSON to type [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md).
- [Write(Utf8JsonWriter, FluentValidationProblemDetails?, JsonSerializerOptions)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationproblemdetailsconverter/methods.md#write-utf8jsonwriter-fluentvalidationproblemdetails-jsonserializeroptions) -- Writes a specified value as JSON.
