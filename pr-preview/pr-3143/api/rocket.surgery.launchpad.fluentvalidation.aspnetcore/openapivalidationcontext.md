# OpenApiValidationContext

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Version: `2.0.1-pr.1161`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi`
- Target framework: `net10.0`
- Implements: `IEquatable<OpenApiValidationContext>`

Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

public record OpenApiValidationContext
    : System.IEquatable<Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi.OpenApiValidationContext>
{
    // ...
}
```

## Constructors

- [OpenApiValidationContext(OpenApiSchema, OpenApiSchema, OpenApiSchemaTransformerContext, IPropertyValidator, string, IRuleComponent)](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/constructors.md#constructor-openapischema-openapischema-openapischematransformercontext-ipropertyvalidator-string-irulecomponent)

## Properties

- [PropertyKey](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/properties.md#propertykey) : `string` `get; init`
- [PropertySchema](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/properties.md#propertyschema) : `OpenApiSchema` `get; init`
- [PropertyValidator](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/properties.md#propertyvalidator) : `IPropertyValidator` `get; init`
- [RuleComponent](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/properties.md#rulecomponent) : `IRuleComponent` `get; init`
- [TransformerContext](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/properties.md#transformercontext) : `OpenApiSchemaTransformerContext` `get; init`
- [TypeSchema](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/properties.md#typeschema) : `OpenApiSchema` `get; init`

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/methods.md#clone) : [OpenApiValidationContext](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md) `virtual`
- [Deconstruct(OpenApiSchema, OpenApiSchema, OpenApiSchemaTransformerContext, IPropertyValidator, string, IRuleComponent)](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/methods.md#deconstruct-openapischema-openapischema-openapischematransformercontext-ipropertyvalidator-string-irulecomponent)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(OpenApiValidationContext?)](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/methods.md#equals-openapivalidationcontext) : `bool` `virtual` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(OpenApiValidationContext?, OpenApiValidationContext?)](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/methods.md#op-equality-openapivalidationcontext-openapivalidationcontext) : `bool` `static`
- [op_Inequality(OpenApiValidationContext?, OpenApiValidationContext?)](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/methods.md#op-inequality-openapivalidationcontext-openapivalidationcontext) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext/methods.md#tostring) : `string` -- Returns a string that represents the current object.
