# RequestAuthorizer Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md)
- Kind: `Methods`
- Members: `3`

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once. A policy sees the caller and nothing of the request: it is the same check wherever it runs, and a policy with scope attributes checks the caller's scopes itself.

<a id="authorizeasync"></a>
<a id="authorizeasync-requestauthorizationplan-cancellationtoken"></a>

## AuthorizeAsync(RequestAuthorizationPlan, CancellationToken)

- Name: `AuthorizeAsync(RequestAuthorizationPlan, CancellationToken)`
- Returns: `ValueTask<AuthorizationDecision>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)

Decides a request's plan for the caller: every policy that has no outcome yet for this call runs.

```csharp
public sealed class RequestAuthorizer
{
    public ValueTask<AuthorizationDecision> AuthorizeAsync(
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

<a id="authorizeasync-policyrequirement-cancellationtoken"></a>

## AuthorizeAsync(PolicyRequirement, CancellationToken)

- Name: `AuthorizeAsync(PolicyRequirement, CancellationToken)`
- Returns: `ValueTask<AuthorizationDecision>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)

Decides one policy for the caller, as the edge does for each policy an endpoint names. Its outcome is recorded, so the plan of the request finds it and the policy doesn't run again.

```csharp
public sealed class RequestAuthorizer
{
    public ValueTask<AuthorizationDecision> AuthorizeAsync(
        PolicyRequirement policy,
        CancellationToken cancellationToken)
    {
        // ...
    }
}
```

## Parameters

- `policy` ([PolicyRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md))
  The policy.
- `cancellationToken` (`CancellationToken`)
  The cancellation token of the call.

<a id="isanonymous"></a>
<a id="isanonymous-isecuritycontext"></a>

## IsAnonymous(ISecurityContext)

- Name: `IsAnonymous(ISecurityContext)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L44-L47)

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
