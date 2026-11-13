# OpenApiValidationContext Constructors

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [OpenApiValidationContext](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md)
- Kind: `Constructors`
- Members: `1`

Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

<a id="constructor"></a>
<a id="constructor-openapischema-openapischema-openapischematransformercontext-ipropertyvalidator-string-irulecomponent"></a>

## OpenApiValidationContext(OpenApiSchema, OpenApiSchema, OpenApiSchemaTransformerContext, IPropertyValidator, string, IRuleComponent)

- Name: `Constructor(OpenApiSchema, OpenApiSchema, OpenApiSchemaTransformerContext, IPropertyValidator, string, IRuleComponent)`

```csharp
public record OpenApiValidationContext
{
    public OpenApiValidationContext(
        OpenApiSchema TypeSchema,
        OpenApiSchema PropertySchema,
        OpenApiSchemaTransformerContext TransformerContext,
        IPropertyValidator PropertyValidator,
        string PropertyKey,
        IRuleComponent RuleComponent)
    {
        // ...
    }
}
```

## Parameters

- `TypeSchema` (`OpenApiSchema`)
- `PropertySchema` (`OpenApiSchema`)
- `TransformerContext` (`OpenApiSchemaTransformerContext`)
- `PropertyValidator` (`IPropertyValidator`)
- `PropertyKey` (`string`)
- `RuleComponent` (`IRuleComponent`)
