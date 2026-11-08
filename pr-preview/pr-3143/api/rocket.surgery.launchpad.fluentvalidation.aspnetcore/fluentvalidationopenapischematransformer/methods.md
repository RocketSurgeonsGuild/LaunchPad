# FluentValidationOpenApiSchemaTransformer Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [FluentValidationOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationopenapischematransformer.md)
- Kind: `Methods`
- Members: `1`

Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

<a id="transformasync"></a>
<a id="transformasync-openapischema-openapischematransformercontext-cancellationtoken"></a>

## TransformAsync(OpenApiSchema, OpenApiSchemaTransformerContext, CancellationToken)

- Name: `TransformAsync(OpenApiSchema, OpenApiSchemaTransformerContext, CancellationToken)`
- Returns: `Task`

Transforms the specified OpenAPI schema.

```csharp
public class FluentValidationOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        // ...
    }
}
```

## Parameters

- `schema` (`OpenApiSchema`)
  The `OpenApi.OpenApiSchema` to modify.
- `context` (`OpenApiSchemaTransformerContext`)
  The `OpenApi.OpenApiSchemaTransformerContext` associated with the .
- `cancellationToken` (`CancellationToken`)
  The cancellation token to use.

## Returns

`Task` -- The task object representing the asynchronous operation.
