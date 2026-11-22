# An operation's policies are declared on its request, answer as union cases and publish their scopes

An operation is authorized by the policies its request names with ZeroAlloc.Authorization's `[RequirePolicy]` and `[RequireAnyPolicy]`. A policy is a `[Policy("Name")]` class, and it declares the scopes it requires with LaunchPad's `[RequiresScopes]`. LaunchPad's own mediator pipeline behavior evaluates them: a caller with no identity answers `Unauthorized` (401), and a caller who lacks a declared scope or fails the policy's own logic answers `Forbidden` (403), each as a case of the operation's union response ([ADR-0006](0006-union-responses-over-every-transport.md)). The scopes a request's policies declare are published as what the operation requires, in each transport's contract where the transport has a place for them.

LaunchPad depends on ZeroAlloc.Authorization for its policies, attributes and security context; it is host-neutral and AOT-safe. LaunchPad writes its own plan for each request, so it can check scopes and record what each policy decided. It does not use ZeroAlloc.Mediator.Authorization, whose behavior can only answer its own result type or throw.

## Decisions

- **The scopes a policy declares are enforced by LaunchPad**, before the policy's own logic runs, so the published contract cannot drift from what is enforced.
- **A request with a policy answers a union** that has both `Unauthorized` and `Forbidden` (`Problem` covers both). Anything else doesn't compile, so a denial is always a union case and the pipeline never throws one, except for a stream, where the mediator gives a behavior no item to answer with: its denial is thrown and each transport writes it as the same case. A handler still answers `Forbidden` for rules of its own.
- **Policies are also enforced at the edge**, before binding: a generated endpoint names each policy of its request by the policy's own name, and ASP.NET Core and HotChocolate authorization run it. The edge and the pipeline share a record of outcomes for the call, so each policy runs once. A group of alternatives has no counterpart at the edge, so the endpoint then only requires a caller and the pipeline decides the group; and where there is no edge (Blazor Server, a worker, a direct dispatch) the pipeline decides everything.
- **A policy sees the caller, never the request.** It is the same check wherever it runs and whichever request names it, which is what lets the edge run it before there is a request. A rule about what a request contains belongs in its handler.
- **An edge rejection is written exactly as the union case would be**, with a `WWW-Authenticate` challenge naming the scopes. A client cannot tell which of the two rejected it.
- **Combined policies are published as alternatives.** Stacked `[RequirePolicy]` attributes are AND and a `[RequireAnyPolicy]` group is OR; the generators expand them into alternatives, each a set of scopes, which is the shape OpenAPI and `@requiresScopes` both use.
- **Per transport:**
    - OpenAPI: a security requirement per alternative on the operation, against a scheme the application owns (named `oauth2` unless configured). LaunchPad never replaces a scheme that is declared. When none is, it declares one, since a requirement of an undeclared scheme reads as "no security": `oauth2` with the authorization code flow and every scope when the application gives it the authorize and token URLs, otherwise a bearer token.
    - GraphQL: `@requiresScopes` on the field (`@authenticated` for a policy without scopes), beside HotChocolate's `@authorize`, which enforces and names a policy, not scopes. LaunchPad declares the two directives, with the names and shapes of the composite schema ones, which HotChocolate 16 does not have.
    - gRPC: nothing in the contract. A rejection carries a `www-authenticate` trailer naming the scopes.
- **A host's `Policy`, `Roles` and `AuthenticationSchemes`** on `[Operation<T>]` stay as they are: authorization of that transport alone, which publishes no scopes.

## Considered Options

- **The policy name is the scope.** Rejected: a policy is also where rules that are not scopes live, and one policy can require several scopes.
- **Use ZeroAlloc.Mediator.Authorization's behavior.** Rejected: on failure it answers `Result<T, AuthorizationFailure>` or throws, and its failure does not tell an unauthenticated caller from a forbidden one. Having unions implement its failure factory would tie every union to ZeroAlloc.Results.
- **Enforce in the pipeline only**, or **at the edge only**. Rejected: the pipeline alone rejects nothing before the request is bound, and the edge alone leaves operations dispatched without a transport unprotected.
- **Derive the OpenAPI security scheme from the JWT bearer options.** Rejected: the authentication configuration doesn't record the grant flow, a document built at build time has no configuration, an `openIdConnect` scheme loses the scope list in Swagger UI, and ASP.NET Core plans its own inference ([dotnet/aspnetcore#39761](https://github.com/dotnet/aspnetcore/issues/39761)).
- **A custom method option in `.proto` for the scopes.** Rejected: gRPC has no standard for scopes on a method (`google.api.oauth_scopes` is for a whole service), and protobuf-net.Grpc cannot write a custom option for a code-first service.

## Consequences

- A new integration package, `Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization`, holds `[RequiresScopes]`, the pipeline behavior and the record of outcomes. The security context built from the caller's `ClaimsPrincipal`, the bridge and each transport's rejection shape go into the existing `ZeroAlloc.AspNetCore`, `ZeroAlloc.Grpc` and `ZeroAlloc.HotChocolate` packages.
- The API generators read a request's policies, and each policy's scopes, from attributes at compile time, including from a referenced assembly.
- The 401 and 403 of an operation with request policies are union cases with a body on every transport, where a host-level `Policy` still answers the framework's bare challenge.
- [docs/design/authorization.md](../design/authorization.md) has the detail.
