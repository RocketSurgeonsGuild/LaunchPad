# ZeroAllocOpenApiSchemaTransformer Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md)
- Kind: `Methods`
- Members: `1`

Describes the ZeroAlloc.Validation attributes of a type's properties in its OpenAPI schema: required properties, lengths, ranges, patterns and e-mail addresses. It also marks the members of the problem details LaunchPad writes as required: the type and title, and the errors of a validation problem.

<a id="transformasync"></a>
<a id="transformasync-openapischema-openapischematransformercontext-cancellationtoken"></a>

## TransformAsync(OpenApiSchema, OpenApiSchemaTransformerContext, CancellationToken)

- Name: `TransformAsync(OpenApiSchema, OpenApiSchemaTransformerContext, CancellationToken)`
- Returns: `Task`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3310-merge.Sha.8860010d83b5d02194514237744998b27c36d06d/src/ZeroAlloc.AspNetCore/OpenApi/ZeroAllocOpenApiSchemaTransformer.cs#L29-L46)

Transforms the specified OpenAPI schema.

```csharp
public sealed class ZeroAllocOpenApiSchemaTransformer
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
