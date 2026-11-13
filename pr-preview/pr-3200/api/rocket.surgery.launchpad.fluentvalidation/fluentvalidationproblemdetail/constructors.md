# FluentValidationProblemDetail Constructors

- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.fluentvalidation.md)
- Type: [FluentValidationProblemDetail](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationproblemdetail.md)
- Kind: `Constructors`
- Members: `2`

A validation error problem

<a id="constructor"></a>

## FluentValidationProblemDetail

- Name: `Constructor`

An empty validation error problem, for deserialization.

```csharp
public class FluentValidationProblemDetail
{
    public FluentValidationProblemDetail()
    {
        // ...
    }
}
```

<a id="constructor-validationfailure"></a>

## FluentValidationProblemDetail(ValidationFailure)

- Name: `Constructor(ValidationFailure)`

A validation error problem

```csharp
public class FluentValidationProblemDetail
{
    public FluentValidationProblemDetail(
        ValidationFailure validationFailure)
    {
        // ...
    }
}
```

## Parameters

- `validationFailure` (`ValidationFailure`)
