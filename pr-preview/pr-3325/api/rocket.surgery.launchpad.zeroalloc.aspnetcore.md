# Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore

- Package: `Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore`
- Version: `2.0.1-pr.1286`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.AspNetCore)
- NuGet: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore)

## Namespaces

### Rocket.Surgery.LaunchPad.AspNetCore.Authorization

- [AuthorizationProblems](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/authorizationproblems.md) -- `class` `static` -- How an `Foundation.Unauthorized` or `Foundation.Forbidden` problem is written over HTTP: as the generated servers write the case.
- [ClaimsSecurityContext](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/claimssecuritycontext.md) -- `class` `sealed` -- A `Claims.ClaimsPrincipal` as a security context.
- [OpenApiAuthorizationOptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions.md) -- `class` `sealed` -- How the scopes of an operation's policies are described in the OpenAPI document.

### Rocket.Surgery.LaunchPad.AspNetCore.Joints

- [ZeroAllocAuthorizationJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocauthorizationjoint.md) -- `class` -- Checks the policies of a request at the edge of an ASP.NET Core application, before it is bound, and describes them in the OpenAPI document (docs/design/authorization.md, sections 4 and 5). Implements the `Joints.IServiceJoint`
- [ZeroAllocOpenApiJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapijoint.md) -- `class` -- Adds the [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md) to every OpenAPI document, so the ZeroAlloc.Validation rules show in the schemas. Implements the `Joints.IServiceJoint`
- [ZeroAllocProblemDetailsJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocproblemdetailsjoint.md) -- `class` -- Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into a 422 with [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md), for controllers and for the exception handler alike. Implements the `Joints.IServiceJoint`

### Rocket.Surgery.LaunchPad.AspNetCore.OpenApi

- [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md) -- `class` `sealed` -- Describes the ZeroAlloc.Validation attributes of a type's properties in its OpenAPI schema: required properties, lengths, ranges, patterns and e-mail addresses. It also marks the members of the problem details LaunchPad writes as required: the type and title, and the errors of a validation problem.

### Rocket.Surgery.LaunchPad.AspNetCore.Validation

- [ValidationFailureDetail](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md) -- `record` -- A single validation failure.
- [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md) -- `class` -- The problem details for a request the ZeroAlloc validators rejected, with the failures grouped by property.
