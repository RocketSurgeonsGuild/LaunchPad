# Authorization: request policies that publish their scopes

Status: **designed and built 2026-11-21** under [#3324](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3324) ([ADR-0011](../adr/0011-request-policies-publish-scopes.md)).

Related: [union-types.md](union-types.md) and [ADR-0006](../adr/0006-union-responses-over-every-transport.md) (error cases), [api-generators-plan.md](api-generators-plan.md) (the generated servers), [ZeroAlloc.Authorization](https://github.com/ZeroAlloc-Net/ZeroAlloc.Authorization) and, as a reference only, [ZeroAlloc.Mediator.Authorization](https://github.com/ZeroAlloc-Net/ZeroAlloc.Mediator/tree/main/src/ZeroAlloc.Mediator.Authorization).

Contents:

1. [What an application writes](#1-what-an-application-writes)
2. [What ZeroAlloc gives us, and what it doesn't](#2-what-zeroalloc-gives-us-and-what-it-doesnt)
3. [Evaluation](#3-evaluation)
4. [The edge](#4-the-edge)
5. [Publishing scopes](#5-publishing-scopes)
6. [Diagnostics](#6-diagnostics)
7. [Packages](#7-packages)
8. [Samples and tests](#8-samples-and-tests)
9. [Decisions](#9-decisions)

---

## 1. What an application writes

A policy, once, beside its operations:

```csharp
[Policy("Rockets.Write"), RequireScope("rockets:write")]
public sealed partial class RocketsWrite : IAuthorizationPolicy;
```

A policy with scope attributes is those scopes and nothing else: LaunchPad writes its `EvaluateAsync`. `[RequireScope("a", "b")]` requires every scope it lists, `[RequireAnyScope("a", "b")]` any one of them, and several attributes on a class must all be satisfied. A policy with a rule of its own is an ordinary ZeroAlloc policy, written by hand, without scope attributes.

The policy on each request it guards, and the two cases in the response:

```csharp
[RequirePolicy("Rockets.Write")]
public readonly record struct Request(RocketId Id) : IRequest<Result>;

public readonly union Result(Unit, NotFound, Unauthorized, Forbidden);
```

Nothing else: the host that lists the operation needs no `Policy`, and the handler doesn't check the caller.

## 2. What ZeroAlloc gives us, and what it doesn't

ZeroAlloc.Authorization 2.3 is host-neutral and AOT-safe. LaunchPad uses its:

- `[Policy("Name")]` classes implementing `IAuthorizationPolicy` (or `IAuthorizationPolicy<T1..T3>` for a policy with arguments);
- `[RequirePolicy(name, args…)]` (stacked attributes are AND) and `[RequireAnyPolicy(names…)]` (an OR group) on a request;
- `ISecurityContext` (`Id`, `Roles`, `Claims`): the caller, which is all a policy sees. LaunchPad does not pass the request (`IResourceSecurityContext<TResource>`), so a policy is the same check wherever it runs; a rule about what a request contains belongs in its handler, which answers `Forbidden` itself;
- diagnostics ZAUTH001 to ZAUTH009. LaunchPad registers each policy a request names itself, as a scoped service, so an application need not call `AddZeroAllocAuthorization()`.

What it lacks, which LaunchPad adds:

| Gap                                                                                          | LaunchPad                                                                                                           |
| -------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| No scopes.                                                                                   | `[RequireScope]` and `[RequireAnyScope]` on the policy class, and its generated `EvaluateAsync`, which checks them. |
| A failure is a free-form code: nothing tells an unauthenticated caller from a forbidden one. | An anonymous security context answers `Unauthorized`; every other denial, `Forbidden`.                              |
| `AuthorizerFor<TRequest>` evaluates a request's policies as a whole.                         | A generated plan per request evaluates policy by policy, so outcomes can be recorded.                               |
| ZeroAlloc.Mediator.Authorization answers `Result<T, AuthorizationFailure>` or throws.        | LaunchPad's behavior answers the union's own case.                                                                  |
| No ASP.NET Core integration.                                                                 | The security context from the caller's `ClaimsPrincipal`, and the edge (section 4).                                 |

## 3. Evaluation

**The plan.** For each request with a policy attribute, in the project that declares it, the generator writes the request's authorization plan into `AddLaunchPadRequestAuthorization()`, which registers it with the application's services:

- its groups, in attribute order: each an AND entry (one policy with its constant arguments) or an OR group (several policies);
- per policy: its name and arguments, the scopes it declares, and a delegate that resolves the policy class from the request's services and calls `EvaluateAsync`;
- how to answer: `Unauthorized` and `Forbidden` converted to the response union.

**The registration.** Everything is a service: nothing is static, and nothing runs when an assembly loads.

- `AddLaunchPadRequestAuthorization()` is written into each project, internal to it. It registers the project's plans (each as `RequestAuthorizationPlan<TRequest>`, and among all `RequestAuthorizationPlan`s for the edge), an `AuthorizationResponse<TResponse>` per response union, and each policy class as a scoped service.
- A joint calls it. The package ships the joint as a template (`src/ZeroAlloc.Authorization/build/LaunchPadRequestAuthorizationJoint.cs.template`), written into every project that references the package and runs Dovetail's generator and LaunchPad's analyzer, the way Foundation ships the joint of `[Mutable]` validators. The generator writes the method only where the joint is. An application without Dovetail calls the method of each assembly itself.
- `RequestAuthorizer` is a scoped service that decides a plan for the call: it is given the call's services, its authorization outcomes and the application's `RequestAuthorizationPlans`, and resolves the caller and the policies from the services.

**The behavior.** `AuthorizationBehavior` is a ZeroAlloc.Mediator pipeline behavior at order `-1000`, ahead of validation. For a request with a plan:

1. The security context is resolved from the request's services. An anonymous one answers `Unauthorized`; no policy runs.
2. Each policy is looked up in the request's **authorization outcomes** (scoped, keyed by policy name and arguments). An outcome that is there is reused.
3. Otherwise its `EvaluateAsync` runs, and the outcome is recorded. A policy with scope attributes checks the caller's scopes there, and its failure names the scopes the caller lacks.
4. A group that fails answers `Forbidden`, with the failure's reason as its detail, and the missing scopes when that was why.

A request without a policy attribute passes straight through. One that has a policy attribute and no plan in the services is refused with an exception that names the missing registration, so a forgotten registration never lets a request through. A stream request is checked by the behavior's `CheckStream`, which the mediator calls before the stream handler; it has no item to answer with, so a denial throws `AuthorizationDeniedException` carrying the problem, and each transport writes it as that case.

**Static dispatch is not supported.** ZeroAlloc.Mediator also generates a static `Mediator` class, which dispatches with no services, so nothing could check a request's policies. LaunchPad bans it: the package ships a banned symbols list (`build/BannedSymbols.Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization.txt`, RS0030 as an error; `LaunchPadAuthorizationBanStaticMediator=false` leaves it out). ZeroAlloc's own generated mediator uses the class, and no analyzer setting reaches one generated file, so LaunchPad's generator writes `SuppressMessage` attributes for the types ZeroAlloc generates. ZeroAlloc's generator calls a `Handle` overload without services on every behavior, so the behavior has one: it refuses a request with a policy and passes any other on.

**The caller's scopes** are the security context's `scope` claim, separated by spaces. The check is the policy's own (`ScopeRequirements.Evaluate`, which the generated `EvaluateAsync` calls), so it runs wherever the policy does: behind a server, in a worker, in a function, or when the policy is called directly. A caller without a required scope is denied with the failure code `scope.missing`, whose reason is the scopes it lacks.

**The security context** is whatever `ISecurityContext` the request's services resolve. `ZeroAlloc.AspNetCore` registers one built from `HttpContext.User`: the name identifier as `Id`, role claims as `Roles`, and the other claims by type, the values of a repeated claim joined by a space. Entra's `scp` claim becomes `scope`, so a policy reads scopes one way whatever issued the token. Without an authenticated identity it is `AnonymousSecurityContext.Instance`. An application without ASP.NET Core registers its own.

## 4. The edge

The generated servers also reject at the edge, before the request is bound, through the transport's own authorization.

- **The bridge.** A generated endpoint names each policy every caller must pass (the `[RequirePolicy]` entries) by its own name: `RequireAuthorization("Rockets.Write")`, `[Authorize(Policy = "Rockets.Write")]`, and HotChocolate's `[Authorize]` for a field. A policy with arguments is named with them (`MinimumClearance(3)`). An `IAuthorizationPolicyProvider` in `ZeroAlloc.AspNetCore` answers the name of a policy some request names with an ASP.NET Core policy that runs it; any other name goes to the default provider. A request's policy and an ASP.NET Core policy of the application with the same name are one name for two things, and the request's policy is the one that answers: giving them different names is the application's to do.
- **The handler** runs the policy through the call's `RequestAuthorizer` and records its outcome. The behavior then finds it and runs nothing twice.
- **A group of alternatives** (`[RequireAnyPolicy]`) has no counterpart at the edge, which can only say "all of these". The endpoint then requires a caller and nothing more (a plain `[Authorize]`), and the group is decided in the pipeline.
- **The answer.** The authorization middleware's result handler lets the authentication schemes challenge or forbid, then writes the body: problem details with `case`, as the generated HTTP servers write the `Unauthorized` or `Forbidden` case, or for a gRPC call the status with the `case` trailer and no message. A 401, and a 403 for missing scopes, carry `WWW-Authenticate: Bearer` with `scope="…"`, and `error="insufficient_scope"` on the 403 ([RFC 6750](https://www.rfc-editor.org/rfc/rfc6750#section-3)); a gRPC client reads it among the trailers. A GraphQL field is rejected by HotChocolate's own authorization, which asks ASP.NET Core for the policy; an error filter gives its error the code and `extensions.case` of the union case.

Where there is no edge (Blazor Server, a worker, a console application, a hand-written endpoint that dispatches the request), nothing is recorded and the behavior evaluates everything.

A host's own `Policy`, `Roles` and `AuthenticationSchemes` on `[Operation<T>]` are unchanged: ASP.NET Core authorization of that transport, answered the framework's way, publishing nothing.

## 5. Publishing scopes

The generators read a request's policy attributes, find each policy class by its `[Policy]` name in the compilation and its references, and take the scopes from its scope attributes: a `[RequireScope]` adds its scopes to every alternative of the policy, a `[RequireAnyScope]` multiplies them by its scopes. The groups are expanded into **alternatives**, each a set of scopes, any one of which is enough:

- AND entries add their scopes to every alternative;
- an OR group multiplies the alternatives by its policies;
- duplicate alternatives, and alternatives that contain another, are dropped.

| Transport | Published as                                                                                                                                                                                                                                                                                                                                                                                         |
| --------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| OpenAPI   | `security` on the operation: one requirement per alternative, `{ oauth2: [scopes] }`. The scheme name is `oauth2` unless `OpenApiAuthorizationOptions.SchemeName` says otherwise. 401 and 403 are described by their union cases. The generated transformer of the operation calls `OpenApiAuthorization.RequireScopes<TRequest>()`, which reads the request's plan from the application's services. |
| GraphQL   | `@requiresScopes(scopes: [[…], […]])` on the field: the outer list is the alternatives. A policy with no scopes publishes `@authenticated`. LaunchPad declares both directives, with the names and shapes of the composite schema ones (HotChocolate 16 has none); they state, and HotChocolate's `@authorize` enforces.                                                                             |
| gRPC      | Nothing: there is no standard for scopes on a method. A rejection names them in the `www-authenticate` trailer.                                                                                                                                                                                                                                                                                      |

**The OpenAPI security scheme** belongs to the application. LaunchPad never replaces one that is declared. When none of that name is declared, a document transformer declares it, because a requirement of a scheme the document doesn't declare is written as `{}`, which reads as "no security":

- with an authorization URL and a token URL in `OpenApiAuthorizationOptions`, as `oauth2` with the authorization code flow, listing every scope the document's operations require (with descriptions from the options);
- without them, as a bearer token (`type: http`, `scheme: bearer`), which a document generated at build time gets.

## 6. Diagnostics

| Id       | Severity | When                                                                                                                                                                                                       |
| -------- | -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| LPAD1050 | Error    | A request with a policy has a response that is not a union, or whose union lacks `Unauthorized` (and isn't covered by `Problem`).                                                                          |
| LPAD1051 | Error    | A request with a policy has a response union that lacks `Forbidden` (and isn't covered by `Problem`).                                                                                                      |
| LPAD1052 | Warning  | A request's policies expand to more than 16 alternatives; the first 16 are published.                                                                                                                      |
| LPAD1053 | Error    | A class with a scope attribute that its `EvaluateAsync` cannot be written for: not a `[Policy]`, not partial, not listing `IAuthorizationPolicy`, declaring `EvaluateAsync` itself, or with a blank scope. |

| LPAD1054 | Error | A request with a policy is generic, private or protected: no plan can be written for it, so its policies would never be checked. |

ZeroAlloc's own ZAUTH001 to ZAUTH009 still report an unknown policy name, a duplicate and the rest.

## 7. Packages

| Package                                            | Gets                                                                                                                                                                                                                                               |
| -------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization` | New, host-neutral: `[RequireScope]`, `[RequireAnyScope]` and the scope check, the plan types, `AuthorizationBehavior`, `RequestAuthorizer`, the authorization outcomes, its joint, and the template of the joint that registers a project's plans. |
| `Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore`    | The security context from `HttpContext.User`, the bridge and its handler, the result handler (HTTP and gRPC), `OpenApiAuthorizationOptions` and the OpenAPI transformers.                                                                          |
| `Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc`          | The interceptor that answers a denial the pipeline threw (a stream request) as a status with its `case` trailer.                                                                                                                                   |
| `Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate`  | `@requiresScopes` and `@authenticated`, HotChocolate's ASP.NET Core authorization, and the error filter for a rejected field.                                                                                                                      |
| `Rocket.Surgery.LaunchPad.Analyzers`               | The plan generator and its diagnostics, and what the API generators name at the edge and state in the GraphQL schema.                                                                                                                              |

The LaunchPad SDK adds the new package to a project that references ZeroAlloc.Authorization directly, and the transport packages as it does today.

## 8. Samples and tests

- `Sample.Core` declares four policies (`Rockets.Read`, `Rockets.Write`, `LaunchRecords.Read`, `LaunchRecords.Write`) with the scopes `rockets:read`, `rockets:write`, `launch-records:read` and `launch-records:write`, and every request requires one.
- `Sample.AppHost` runs Keycloak (Aspire's Keycloak resource) with the realm in `Realms/launchpad-realm.json`: the four scopes, a public client for a caller who signs in (`sample-web`, authorization code with PKCE), a confidential client for a service that calls an API (`sample-service`, client credentials), and two users. It is for running the samples locally.
- The API samples (Restful, Classic.Restful, Minimal, Grpc, Graphql) validate the realm's access tokens (`sample/Shared/SampleAuthentication.cs`). Swagger UI signs in with the public client, through the `oauth2` scheme LaunchPad declares from the realm's endpoints.
- The samples with pages (Pages, BlazorServer) sign a caller in with OpenID Connect and a cookie (`sample/Shared/SampleSignIn.cs`). Blazor Server dispatches from a circuit, where there is no request, so it registers a security context over the circuit's signed-in user.
- `Sample.Worker` dispatches with no caller at all: it registers itself as the caller, with the one scope it needs.
- Not covered: the hand-written GraphQL queries of `Sample.Graphql` read the database without a request, so no policy guards them; `Sample.BlazorWasm` has requests of its own, without policies.
- Tests need no Keycloak. `test/TestSupport` has `TestTokens`: the web fixtures make JWT bearer trust a signing key of the test process, and give a request without an `Authorization` header the token of a default caller with every scope, so existing tests run as that caller. A test of a denial sends its own token, or the `X-Test-Anonymous` header. A handler test, which has no transport, registers `TestCaller`.
- `test/Sample.AppHost.Tests` is the one suite that does use Keycloak: it starts `Sample.AppHost` on Aspire's testing host with Keycloak and the Minimal API and GraphQL samples, and tests client credentials, the authorization code flow with PKCE as an API explorer runs it from the browser, and the scheme in the OpenAPI document. It needs Docker and is skipped without it.

## 9. Decisions

Decided 2026-11-21, with the reasons in [ADR-0011](../adr/0011-request-policies-publish-scopes.md):

1. Keep `Unauthorized` as the 401 case; `Forbidden` already existed.
2. A policy declares its scopes with `[RequireScope]` and `[RequireAnyScope]`, and is those scopes and nothing else: LaunchPad writes the `EvaluateAsync` that checks them, so the check runs wherever the policy does (revised 2026-11-22; LaunchPad first checked them itself, ahead of the policy, which only held where LaunchPad dispatched the request).
3. Enforced in the pipeline and at the edge, each policy once, through shared authorization outcomes.
4. A request with a policy answers a union with `Unauthorized` and `Forbidden`.
5. Use ZeroAlloc.Authorization, not ZeroAlloc.Mediator.Authorization.
6. An edge rejection has the union case's shape.
7. Policies are published as alternatives of scopes; `@requiresScopes` for GraphQL, trailers only for gRPC.
8. The application owns the OpenAPI security scheme; LaunchPad's `oauth2` scheme is opt-in.
9. Every sample operation is protected, with Keycloak through Aspire; tests use a signing key of their own.
10. An operation's security scheme is always declared in the OpenAPI document: a requirement of an undeclared scheme is written as `{}`, which reads as "no security" (found while building).
11. Plans are registered with the application's services by a joint written from a template, not from a module initializer: the behavior, the edge and the policies all get what they need from the services (decided 2026-11-22).
12. A policy sees the caller only, never the request, and is named at the edge by its own name (decided 2026-11-22). A name shared with an ASP.NET Core policy of the application is the application's to resolve.
13. ZeroAlloc.Mediator's static `Mediator` is banned, with the ban suppressed for the code ZeroAlloc generates (decided 2026-11-22).
14. `@requiresScopes` and `@authenticated` are declared by LaunchPad: HotChocolate 16 does not have the composite schema directives (found while building).
