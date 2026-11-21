# RequestAuthorizer Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md)
- Kind: `Methods`
- Members: `4`

Decides a request's plan for a caller, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. The mediator pipeline and the edge both decide through it.

<a id="authorizeasync"></a>
<a id="authorizeasync-requestauthorizationplan-trequest-trequest-iserviceprovider-cancellationtoken"></a>

## AuthorizeAsync(RequestAuthorizationPlan<TRequest>, TRequest, IServiceProvider, CancellationToken)

- Name: `AuthorizeAsync(RequestAuthorizationPlan<TRequest>, TRequest, IServiceProvider, CancellationToken)`
- Modifiers: `static`
- Returns: `ValueTask<AuthorizationDecision>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L59-L62)

Decides the plan with the request: every policy that has no outcome yet runs.

```csharp
public static class RequestAuthorizer
{
    public static ValueTask<AuthorizationDecision> AuthorizeAsync<TRequest>(
        RequestAuthorizationPlan<TRequest> plan,
        TRequest request,
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        // ...
    }
}
```

## Parameters

- `plan` ([RequestAuthorizationPlan<TRequest>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1.md))
  The request's plan.
- `request` (`TRequest`)
  The request.
- `services` (`IServiceProvider`)
  The services of the call.
- `cancellationToken` (`CancellationToken`)
  The cancellation token of the call.

<a id="authorizeattheedgeasync"></a>
<a id="authorizeattheedgeasync-requestauthorizationplan-iserviceprovider-cancellationtoken"></a>

## AuthorizeAtTheEdgeAsync(RequestAuthorizationPlan, IServiceProvider, CancellationToken)

- Name: `AuthorizeAtTheEdgeAsync(RequestAuthorizationPlan, IServiceProvider, CancellationToken)`
- Modifiers: `static`
- Returns: `ValueTask<AuthorizationDecision>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L74-L77)

Decides the plan at the edge, before there is a request. A policy that reads the request is left for the pipeline, so the decision denies only what is certain.

```csharp
public static class RequestAuthorizer
{
    public static ValueTask<AuthorizationDecision> AuthorizeAtTheEdgeAsync(
        RequestAuthorizationPlan plan,
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        // ...
    }
}
```

## Parameters

- `plan` ([RequestAuthorizationPlan](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md))
  The request's plan.
- `services` (`IServiceProvider`)
  The services of the call.
- `cancellationToken` (`CancellationToken`)
  The cancellation token of the call.

<a id="isanonymous"></a>
<a id="isanonymous-isecuritycontext"></a>

## IsAnonymous(ISecurityContext)

- Name: `IsAnonymous(ISecurityContext)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L44)

Whether the caller has no identity.

```csharp
public static class RequestAuthorizer
{
    public static bool IsAnonymous(
        ISecurityContext context)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ISecurityContext`)
  The caller.

<a id="securitycontext"></a>
<a id="securitycontext-iserviceprovider"></a>

## SecurityContext(IServiceProvider)

- Name: `SecurityContext(IServiceProvider)`
- Modifiers: `static`
- Returns: `ISecurityContext`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L39)

The caller of the call these services belong to; anonymous when none is registered.

```csharp
public static class RequestAuthorizer
{
    public static ISecurityContext SecurityContext(
        IServiceProvider services)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceProvider`)
  The services of the call.
