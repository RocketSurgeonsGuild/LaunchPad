# FluentValidationOpenApiSchemaTransformer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Version: `2.0.1-pr.1174`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi`
- Target framework: `net10.0`
- Implements: `IOpenApiSchemaTransformer`

Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

public class FluentValidationOpenApiSchemaTransformer
    : Microsoft.AspNetCore.OpenApi.IOpenApiSchemaTransformer
{
    // ...
}
```

## Constructors

- [FluentValidationOpenApiSchemaTransformer(IEnumerable<IPropertyRuleHandler>)](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationopenapischematransformer/constructors.md#constructor-ienumerable-ipropertyrulehandler)

## Methods

- [TransformAsync(OpenApiSchema, OpenApiSchemaTransformerContext, CancellationToken)](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationopenapischematransformer/methods.md#transformasync-openapischema-openapischematransformercontext-cancellationtoken) : `Task` -- Transforms the specified OpenAPI schema.
