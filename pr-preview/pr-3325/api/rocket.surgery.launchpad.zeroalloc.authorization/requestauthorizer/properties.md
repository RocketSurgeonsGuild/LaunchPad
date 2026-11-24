# RequestAuthorizer Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md)
- Kind: `Properties`
- Members: `1`

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once. A policy sees the caller and nothing of the request: it is the same check wherever it runs, and a policy with scope attributes checks the caller's scopes itself.

<a id="caller"></a>

## Caller

- Name: `Caller`
- Modifiers: `get`
- Returns: `ISecurityContext`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L38)

The caller of this call; anonymous when the application registered no `Authorization.ISecurityContext`.

```csharp
public ISecurityContext Caller { get; }
```
