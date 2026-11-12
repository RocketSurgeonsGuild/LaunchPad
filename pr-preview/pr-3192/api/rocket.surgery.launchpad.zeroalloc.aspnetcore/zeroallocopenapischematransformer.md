# ZeroAllocOpenApiSchemaTransformer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1180`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.OpenApi`
- Target framework: `net10.0`
- Implements: `IOpenApiSchemaTransformer`

Describes the ZeroAlloc.Validation attributes of a type's properties in its OpenAPI schema: required properties, lengths, ranges, patterns and e-mail addresses. It also marks the members of the problem details LaunchPad writes as required: the type and title, and the errors of a validation problem.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

public sealed class ZeroAllocOpenApiSchemaTransformer
    : Microsoft.AspNetCore.OpenApi.IOpenApiSchemaTransformer
{
    // ...
}
```

## Remarks

The attributes are read from the members System.Text.Json serializes, through each property's attribute provider, so the schema property and its rules are matched by the serialized name. Rules that cannot be described ( `[Must]`, custom attributes) are left out.

## Constructors

- [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer/constructors.md#constructor)

## Methods

- [TransformAsync(OpenApiSchema, OpenApiSchemaTransformerContext, CancellationToken)](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer/methods.md#transformasync-openapischema-openapischematransformercontext-cancellationtoken) : `Task` -- Transforms the specified OpenAPI schema.
