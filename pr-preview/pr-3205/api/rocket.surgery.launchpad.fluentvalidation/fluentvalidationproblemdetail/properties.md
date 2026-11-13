# FluentValidationProblemDetail Properties

- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation.md)
- Type: [FluentValidationProblemDetail](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationproblemdetail.md)
- Kind: `Properties`
- Members: `5`

A validation error problem

<a id="attemptedvalue"></a>

## AttemptedValue

- Name: `AttemptedValue`
- Modifiers: `nullable` `get; set`
- Returns: `object?`

The property value that caused the failure.

```csharp
public object? AttemptedValue { get; set; }
```

<a id="errorcode"></a>

## ErrorCode

- Name: `ErrorCode`
- Modifiers: `get; set`
- Returns: `string`

Gets or sets the error code.

```csharp
public string ErrorCode { get; set; }
```

<a id="errormessage"></a>

## ErrorMessage

- Name: `ErrorMessage`
- Modifiers: `get; set`
- Returns: `string`

The error message

```csharp
public string ErrorMessage { get; set; }
```

<a id="propertyname"></a>

## PropertyName

- Name: `PropertyName`
- Modifiers: `get; set`
- Returns: `string`

The name of the property.

```csharp
public string PropertyName { get; set; }
```

<a id="severity"></a>

## Severity

- Name: `Severity`
- Modifiers: `get; set`
- Returns: `Severity`

Custom severity level associated with the failure.

```csharp
public Severity Severity { get; set; }
```
