# Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate

- Package: `Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate`
- Version: `2.0.1-pr.1291`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.HotChocolate)
- NuGet: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate)

## Namespaces

### Rocket.Surgery.LaunchPad.HotChocolate.Authorization

- [AuthenticatedDirective](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authenticateddirective.md) -- `class` `sealed` -- The `@authenticated` directive of a field: the caller must be authenticated, with no particular scope.
- [AuthenticatedDirectiveType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authenticateddirectivetype.md) -- `class` `sealed` -- The type of `@authenticated`. It states what is required; the field's authorization enforces it.
- [AuthorizationErrorFilter](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authorizationerrorfilter.md) -- `class` `sealed` -- Reports a field HotChocolate's authorization rejected as the `Unauthorized` or `Forbidden` case of a union response is reported: `UNAUTHENTICATED` or `FORBIDDEN` as the code, and the case name in `extensions.case`. A client reads one shape, whether the edge or the mediator pipeline rejected the call.
- [GraphQLAuthenticatedAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/graphqlauthenticatedattribute.md) -- `class` `sealed` -- States that a field requires an authenticated caller with `@authenticated`. The generated GraphQL hosts put it on the field of an operation whose request has policies that declare no scopes.
- [GraphQLRequiresScopesAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/graphqlrequiresscopesattribute.md) -- `class` `sealed` -- States the scopes a field requires with `@requiresScopes`. The generated GraphQL hosts put it on the field of an operation whose request has policies that declare scopes.
- [RequiresScopesDirective](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/requiresscopesdirective.md) -- `class` `sealed` -- The `@requiresScopes` directive of a field: the scopes a caller needs, as alternatives. The caller needs every scope of any one inner list. It has the name and shape of the composite schema directive, so gateways and tools that know that one read it.
- [RequiresScopesDirectiveType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/requiresscopesdirectivetype.md) -- `class` `sealed` -- The type of `@requiresScopes(scopes: [[String!]!]!)`. It states what is required; the field's authorization enforces it.

### Rocket.Surgery.LaunchPad.HotChocolate.Joints

- [ZeroAllocHotChocolateAuthorizationJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallochotchocolateauthorizationjoint.md) -- `class` -- Authorizes GraphQL fields through ASP.NET Core authorization, where the policies of a field's request are checked, states the scopes a field requires in the schema ( `@requiresScopes`, `@authenticated`), and reports a rejected field as the `Unauthorized` or `Forbidden` case is reported (docs/design/authorization.md). Implements the `Joints.IServiceJoint`
- [ZeroAllocHotChocolateJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallochotchocolatejoint.md) -- `class` -- Reports each failure of a request the ZeroAlloc mediator rejected as a GraphQL `VALIDATION` error. Implements the `Joints.IServiceJoint`

### Rocket.Surgery.LaunchPad.HotChocolate.Validation

- [ZeroAllocValidationErrorFilter](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallocvalidationerrorfilter.md) -- `class` `sealed` -- Reports each failure of a request the ZeroAlloc validators rejected as a `VALIDATION` error, with the property name and the error code as extensions.
