# ValidationProblemDetailsConverter Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [ValidationProblemDetailsConverter](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationproblemdetailsconverter.md)
- Kind: `Methods`
- Members: `2`

A RFC 7807 compliant `Serialization.JsonConverter` for [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md).

<a id="read"></a>
<a id="read-utf8jsonreader-type-jsonserializeroptions"></a>

## Read(Utf8JsonReader, Type, JsonSerializerOptions)

- Name: `Read(Utf8JsonReader, Type, JsonSerializerOptions)`
- Modifiers: `override`
- Returns: [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md)

Reads and converts the JSON to type [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md).

```csharp
public sealed class ValidationProblemDetailsConverter
{
    public override FluentValidationProblemDetails Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `reader` (`Utf8JsonReader`)
  The reader.
- `typeToConvert` (`Type`)
  The type to convert.
- `options` (`JsonSerializerOptions`)
  An object that specifies serialization options to use.

## Returns

[FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md) -- The converted value.

<a id="write"></a>
<a id="write-utf8jsonwriter-fluentvalidationproblemdetails-jsonserializeroptions"></a>

## Write(Utf8JsonWriter, FluentValidationProblemDetails?, JsonSerializerOptions)

- Name: `Write(Utf8JsonWriter, FluentValidationProblemDetails?, JsonSerializerOptions)`
- Modifiers: `override`

Writes a specified value as JSON.

```csharp
public sealed class ValidationProblemDetailsConverter
{
    public override void Write(
        Utf8JsonWriter writer,
        FluentValidationProblemDetails? value,
        JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `writer` (`Utf8JsonWriter`)
  The writer to write to.
- `value` ([FluentValidationProblemDetails?](/LaunchPad/pr-preview/pr-3197/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md))
  The value to convert to JSON.
- `options` (`JsonSerializerOptions`)
  An object that specifies serialization options to use.
