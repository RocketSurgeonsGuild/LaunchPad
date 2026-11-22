# Authorization

An operation is authorized by the **policies** its request names. A policy declares the **scopes** it requires, LaunchPad checks them
wherever the request is dispatched, and each transport's contract states them, so a client knows what to ask its identity provider for.

LaunchPad builds this on [ZeroAlloc.Authorization](https://github.com/ZeroAlloc-Net/ZeroAlloc.Authorization) and ZeroAlloc.Mediator.
The reasons are in
[ADR-0011](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0011-request-policies-publish-scopes.md), and the
detail in the [design page](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/design/authorization.md).

## Packages

| The project                             | References                                                                                             |
| --------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| declares requests, handlers or policies | `ZeroAlloc.Authorization` and `Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization`                       |
| is an ASP.NET Core application          | `Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore`                                                        |
| serves gRPC or GraphQL                  | `Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc` or `Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate` as well |

The [LaunchPad SDK](../concepts/launchpad-sdk/) adds the LaunchPad packages to a project that references `ZeroAlloc.Authorization`.

LaunchPad writes a [Joint](../concepts/joints/) into each of those projects that registers the authorization plans of its requests, so
an application that calls `ConfigureDovetail()` has nothing to register. An application without Dovetail calls
`services.AddLaunchPadRequestAuthorization()` (in `Rocket.Surgery.LaunchPad.Authorization.Generated`) from each project that
declares requests with policies. A request with a policy and no registered plan is refused with an exception, never let through.

Dispatch requests through `IMediator`, resolved from the scope of the call. ZeroAlloc.Mediator's static `Mediator` class dispatches
without services, so no policy could be checked: LaunchPad bans it (RS0030, an error) in a project that references the package.

## Declare a policy

A policy is a class with a name. `[RequiresScopes]` lists the scopes it requires of the caller, all of them:

```csharp
[Policy("Rockets.Write"), RequiresScopes("rockets:write")]
public sealed class RocketsWrite : IAuthorizationPolicy
{
    public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default) =>
        new(UnitResult<AuthorizationFailure>.Success());
}
```

LaunchPad checks the scopes before the policy runs, so a policy that only requires scopes has nothing to decide. A policy with a rule
of its own returns a failure with a reason. A policy sees the caller and nothing of the request, so it is the same check for
every request that names it; a rule about what a request contains belongs in its handler, which answers `Forbidden` itself.

## Require it on a request

```csharp
[RequirePolicy("Rockets.Write")]
public readonly record struct Request(RocketId Id) : IRequest<Result>;

public readonly union Result(Unit, NotFound, Unauthorized, Forbidden);
```

- Stacked `[RequirePolicy]` attributes must all pass. `[RequireAnyPolicy("A", "B")]` passes when any one does.
- The response is a union with `Unauthorized` and `Forbidden` (or `Problem`, which covers both). A caller with no identity is
  answered `Unauthorized`; a caller who lacks a scope, or whom the policy denies, is answered `Forbidden`. The handler doesn't run.
- A stream request has no response to answer with: its denial is thrown as `AuthorizationDeniedException`, and each transport
  answers it as the same case.

| Diagnostic | When                                                                                        |
| ---------- | ------------------------------------------------------------------------------------------- |
| LPAD1050   | The response of a request with a policy is not a union, or its union has no `Unauthorized`. |
| LPAD1051   | Its union has no `Forbidden`.                                                               |
| LPAD1052   | Its policies expand to more than 16 alternative sets of scopes.                             |
| LPAD1053   | `[RequiresScopes]` is on a type that is not a `[Policy]`, or a scope is blank.              |
| LPAD1054   | The request is generic, private or protected, so its policies could not be checked.         |

## Who the caller is

Policies see an `ISecurityContext`: an id, roles and claims. The scopes are read from its `scope` claim (and `scp`), separated by
spaces; `RequestAuthorizationOptions.ScopeClaims` changes that.

- In an ASP.NET Core application the caller is the one ASP.NET Core authenticated. Add authentication as usual, for example JWT bearer.
- Anywhere else (a worker, a console application, a Blazor circuit) register your own `ISecurityContext` as a scoped service.

## What each transport does

The generated servers check a request's policies twice over, and run each policy once: at the **edge**, before the request is
bound, and in the mediator pipeline.

At the edge an operation carries the policies of its request by name, as `[Authorize(Policy = "Rockets.Write")]` would, and LaunchPad
answers those names. Don't give an ASP.NET Core policy of your own the name of a request's policy: the request's policy is the one
that answers. A `[RequireAnyPolicy]` group has no counterpart at the edge, so the operation then only requires a caller there and the
pipeline decides the group.

| Transport | A denial is answered as                                                          | The scopes are published as                                           |
| --------- | -------------------------------------------------------------------------------- | --------------------------------------------------------------------- |
| HTTP      | 401 or 403 problem details with `case`, and `WWW-Authenticate` naming the scopes | `security` on the OpenAPI operation, one requirement per alternative  |
| GraphQL   | an error with the code `UNAUTHENTICATED` or `FORBIDDEN` and `extensions.case`    | `@requiresScopes` on the field (`@authenticated` when there are none) |
| gRPC      | `Unauthenticated` or `PermissionDenied`, with the `case` trailer                 | nothing: gRPC has no place for them; a denial names them in a trailer |

A host's own `Policy`, `Roles` and `AuthenticationSchemes` on `[Operation<T>]` still work. They are ASP.NET Core authorization for
that transport only, and publish no scopes.

## The OpenAPI security scheme

Operations require their scopes of a security scheme named `oauth2`, unless `OpenApiAuthorizationOptions.SchemeName` says otherwise.
The scheme is yours to declare. When you declare none, LaunchPad does:

```csharp
builder.Services.Configure<OpenApiAuthorizationOptions>(options =>
{
    options.AuthorizationUrl = new("https://id.example.com/authorize");
    options.TokenUrl = new("https://id.example.com/token");
    options.ScopeDescriptions["rockets:write"] = "Create, change and remove rockets";
});
```

With both URLs it is OAuth 2.0 with the authorization code flow and every scope the operations require, which is what an API
explorer needs to sign in. Without them it is a bearer token.

## In the samples

`Sample.Core` requires a policy on every request. `Sample.AppHost` runs Keycloak with a realm that has the four scopes, a public
client (`sample-web`), a confidential client (`sample-service`) and two users, `alice` and `bob`, whose passwords are their names.
The API samples validate its access tokens, and the page samples sign in with it. The realm is for running the samples locally.