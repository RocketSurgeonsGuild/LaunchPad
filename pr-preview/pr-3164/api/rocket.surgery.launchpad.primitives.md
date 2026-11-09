# Rocket.Surgery.LaunchPad.Primitives

- Package: `Rocket.Surgery.LaunchPad.Primitives`
- Version: `2.0.1-pr.1170`
- Target framework: `net10.0`
- NuGet: [Rocket.Surgery.LaunchPad.Primitives](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Primitives)

## Namespaces

### Rocket.Surgery.LaunchPad.Primitives

- [Conflict](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/conflict.md) -- `record` `sealed` -- The request conflicts with the current state. HTTP 409, gRPC `FailedPrecondition`, GraphQL `CONFLICT`.
- [ExistingValueOptions](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/existingvalueoptions.md) -- `class` `static` -- Helper methods for creating `Primitives.ExistingValueOptionsFactory`1` instances.
- [ExistingValueOptionsFactory<TOptions>](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/existingvalueoptionsfactory-1.md) -- `class` -- Implementation of `Options.IOptionsFactory`1`.
- [Forbidden](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/forbidden.md) -- `record` `sealed` -- The caller may not do this. HTTP 403, gRPC `PermissionDenied`, GraphQL `FORBIDDEN`.
- [Invalid](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/invalid.md) -- `record` `sealed` -- The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.
- [IProblemDetailsData](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata.md) -- `interface` -- A common interface used for definiting problems
- [NotAuthorizedException](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/notauthorizedexception.md) -- `class` -- NotAuthorizedException.
- [NotFound](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/notfound.md) -- `record` `sealed` -- The requested entity doesn't exist. HTTP 404, gRPC `NotFound`, GraphQL `NOT_FOUND`.
- [NotFoundException](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/notfoundexception.md) -- `class` -- NotFoundException.
- [Problem](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/problem.md) -- `record` `abstract` -- The base of LaunchPad's standard error cases: what a handler returns, as a case of its result union, instead of throwing. Each transport turns a case into its own error shape (HTTP ProblemDetails, a gRPC status, a GraphQL error) from [IProblemDetailsData](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata.md).
- [ProblemDetailsException](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/problemdetailsexception.md) -- `class` `abstract` -- NotFoundException.
- [RequestFailedException](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/requestfailedexception.md) -- `class` -- An exception for failed requests, with overloads to take additional details
- [Unauthorized](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/unauthorized.md) -- `record` `sealed` -- The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.
- [UnprocessableEntity](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/unprocessableentity.md) -- `record` `sealed` -- The request is well formed but fails validation: what FluentValidation reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

### Rocket.Surgery.LaunchPad.Primitives.Extensions

- [UsefulEnumerableExtensions](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/usefulenumerableextensions.md) -- `class` `static` -- Useful extensions for enumerations.

### Rocket.Surgery.LaunchPad.Primitives.Joints

- [DefaultJoint](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/defaultjoint.md) -- `class` -- The joint every LaunchPad application starts from. Implements the `Joints.IServiceJoint`
- [ServiceDiscoveryCoreJoint](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/servicediscoverycorejoint.md) -- `class` -- A joint that registers service discovery and adds it to every `Http.HttpClient` the application creates.
- [StandardResilienceJoint](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/standardresiliencejoint.md) -- `class` -- A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.
- [SystemTextJsonJoint](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/systemtextjsonjoint.md) -- `class` -- A joint that configures `Json.JsonSerializerOptions` the LaunchPad way: camel case names and camel case enums. Implements the `Joints.IServiceJoint`
