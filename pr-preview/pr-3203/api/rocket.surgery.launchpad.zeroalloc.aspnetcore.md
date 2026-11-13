# Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore

- Package: `Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore`
- Version: `2.0.1-pr.1182`
- Target framework: `net10.0`
- NuGet: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore)

## Namespaces

### Rocket.Surgery.LaunchPad.AspNetCore.Joints

- [ZeroAllocOpenApiJoint](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapijoint.md) -- `class` -- Adds the [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md) to every OpenAPI document, so the ZeroAlloc.Validation rules show in the schemas. Implements the `Joints.IServiceJoint`
- [ZeroAllocProblemDetailsJoint](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocproblemdetailsjoint.md) -- `class` -- Turns the `Validation.ValidationFailedException` the ZeroAlloc mediator throws for an invalid request into a 422 with [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md), for controllers and for the exception handler alike. Implements the `Joints.IServiceJoint`

### Rocket.Surgery.LaunchPad.AspNetCore.OpenApi

- [ZeroAllocOpenApiSchemaTransformer](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocopenapischematransformer.md) -- `class` `sealed` -- Describes the ZeroAlloc.Validation attributes of a type's properties in its OpenAPI schema: required properties, lengths, ranges, patterns and e-mail addresses. It also marks the members of the problem details LaunchPad writes as required: the type and title, and the errors of a validation problem.

### Rocket.Surgery.LaunchPad.AspNetCore.Validation

- [ValidationFailureDetail](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md) -- `record` -- A single validation failure.
- [ValidationFailureProblemDetails](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailureproblemdetails.md) -- `class` -- The problem details for a request the ZeroAlloc validators rejected, with the failures grouped by property.
