# OpenApiAuthorization Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [OpenApiAuthorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorization.md)
- Kind: `Methods`
- Members: `1`

What the generated servers' OpenAPI transformers call to describe the authorization of an operation.

<a id="requirescopes"></a>
<a id="requirescopes-openapioperation-openapioperationtransformercontext"></a>

## RequireScopes(OpenApiOperation, OpenApiOperationTransformerContext)

- Name: `RequireScopes(OpenApiOperation, OpenApiOperationTransformerContext)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.AspNetCore/Authorization/OpenApiAuthorization.cs#L44-L54)

States the scopes an operation requires, from the plan of the request it dispatches: a security requirement per alternative, any one of which is enough (docs/design/authorization.md, section 5). A request without a plan adds nothing.

```csharp
public static class OpenApiAuthorization
{
    public static void RequireScopes<TRequest>(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context)
    {
        // ...
    }
}
```

## Parameters

- `operation` (`OpenApiOperation`)
  The operation.
- `context` (`OpenApiOperationTransformerContext`)
  The context of the operation's transformer.
