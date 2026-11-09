# FluentValidationProblemDetails Constructors

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3165/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3165/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md)
- Kind: `Constructors`
- Members: `3`

Problem Details for Fluent Validation

<a id="constructor"></a>

## FluentValidationProblemDetails

- Name: `Constructor`

Construct the Fluent Validation Problem Details

```csharp
public class FluentValidationProblemDetails
{
    public FluentValidationProblemDetails()
    {
        // ...
    }
}
```

<a id="constructor-validationresult"></a>

## FluentValidationProblemDetails(ValidationResult)

- Name: `Constructor(ValidationResult)`

Build Fluent Validation Problem Details from a `Results.ValidationResult`

```csharp
public class FluentValidationProblemDetails
{
    public FluentValidationProblemDetails(
        ValidationResult result)
    {
        // ...
    }
}
```

## Parameters

- `result` (`ValidationResult`)

<a id="constructor-ienumerable-validationfailure"></a>

## FluentValidationProblemDetails(IEnumerable<ValidationFailure>)

- Name: `Constructor(IEnumerable<ValidationFailure>)`

Build Fluent Validation Problem Details from a `Generic.IEnumerable`1`

```csharp
public class FluentValidationProblemDetails
{
    public FluentValidationProblemDetails(
        IEnumerable<ValidationFailure> errors)
    {
        // ...
    }
}
```

## Parameters

- `errors` (`IEnumerable<ValidationFailure>`)
