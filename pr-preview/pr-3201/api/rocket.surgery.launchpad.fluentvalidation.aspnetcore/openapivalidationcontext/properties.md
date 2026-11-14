# OpenApiValidationContext Properties

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [OpenApiValidationContext](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md)
- Kind: `Properties`
- Members: `6`

Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

<a id="propertykey"></a>

## PropertyKey

- Name: `PropertyKey`
- Modifiers: `get; init`
- Returns: `string`

```csharp
public string PropertyKey { get; init; }
```

<a id="propertyschema"></a>

## PropertySchema

- Name: `PropertySchema`
- Modifiers: `get; init`
- Returns: `OpenApiSchema`

```csharp
public OpenApiSchema PropertySchema { get; init; }
```

<a id="propertyvalidator"></a>

## PropertyValidator

- Name: `PropertyValidator`
- Modifiers: `get; init`
- Returns: `IPropertyValidator`

```csharp
public IPropertyValidator PropertyValidator { get; init; }
```

<a id="rulecomponent"></a>

## RuleComponent

- Name: `RuleComponent`
- Modifiers: `get; init`
- Returns: `IRuleComponent`

```csharp
public IRuleComponent RuleComponent { get; init; }
```

<a id="transformercontext"></a>

## TransformerContext

- Name: `TransformerContext`
- Modifiers: `get; init`
- Returns: `OpenApiSchemaTransformerContext`

```csharp
public OpenApiSchemaTransformerContext TransformerContext { get; init; }
```

<a id="typeschema"></a>

## TypeSchema

- Name: `TypeSchema`
- Modifiers: `get; init`
- Returns: `OpenApiSchema`

```csharp
public OpenApiSchema TypeSchema { get; init; }
```
