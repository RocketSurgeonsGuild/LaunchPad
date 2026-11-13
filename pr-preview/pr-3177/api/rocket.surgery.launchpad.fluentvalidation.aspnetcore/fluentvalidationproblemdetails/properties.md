# FluentValidationProblemDetails Properties

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md)
- Kind: `Properties`
- Members: `2`

Problem Details for Fluent Validation

<a id="rules"></a>

## Rules

- Name: `Rules`
- Modifiers: `get; set`
- Returns: `IEnumerable<string>`

The rules run with the validation

```csharp
public IEnumerable<string> Rules { get; set; }
```

<a id="validationerrors"></a>

## ValidationErrors

- Name: `ValidationErrors`
- Modifiers: `get`
- Returns: `IDictionary<string, FluentValidationProblemDetail[]>`

Gets the validation errors associated with this instance of `Foundation.FluentValidationProblemDetail`.

```csharp
public IDictionary<string, FluentValidationProblemDetail[]> ValidationErrors { get; }
```
