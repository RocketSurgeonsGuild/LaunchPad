# RequestAuthorizer Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md)
- Kind: `Methods`
- Members: `3`

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once.

<a id="authorizeasync"></a>
<a id="authorizeasync-requestauthorizationplan-trequest-trequest-cancellationtoken"></a>

## AuthorizeAsync(RequestAuthorizationPlan<TRequest>, TRequest, CancellationToken)

- Name: `AuthorizeAsync(RequestAuthorizationPlan<TRequest>, TRequest, CancellationToken)`
- Returns: `ValueTask<AuthorizationDecision>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L63-L64)

Decides the plan with the request: every policy that has no outcome yet runs.

```csharp
public sealed class RequestAuthorizer
{
    public ValueTask<AuthorizationDecision> AuthorizeAsync<TRequest>(
        RequestAuthorizationPlan<TRequest> plan,
        TRequest request,
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
- `cancellationToken` (`CancellationToken`)
  The cancellation token of the call.

<a id="authorizeattheedgeasync"></a>
<a id="authorizeattheedgeasync-requestauthorizationplan-cancellationtoken"></a>

## AuthorizeAtTheEdgeAsync(RequestAuthorizationPlan, CancellationToken)

- Name: `AuthorizeAtTheEdgeAsync(RequestAuthorizationPlan, CancellationToken)`
- Returns: `ValueTask<AuthorizationDecision>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L75-L76)

Decides the plan at the edge, before there is a request. A policy that reads the request is left for the pipeline, so the decision denies only what is certain.

```csharp
public sealed class RequestAuthorizer
{
    public ValueTask<AuthorizationDecision> AuthorizeAtTheEdgeAsync(
        RequestAuthorizationPlan plan,
        CancellationToken cancellationToken)
    {
        // ...
    }
}
```

## Parameters

- `plan` ([RequestAuthorizationPlan](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md))
  The request's plan.
- `cancellationToken` (`CancellationToken`)
  The cancellation token of the call.

<a id="isanonymous"></a>
<a id="isanonymous-isecuritycontext"></a>

## IsAnonymous(ISecurityContext)

- Name: `IsAnonymous(ISecurityContext)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L50-L53)

Whether the caller has no identity.

```csharp
public sealed class RequestAuthorizer
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
