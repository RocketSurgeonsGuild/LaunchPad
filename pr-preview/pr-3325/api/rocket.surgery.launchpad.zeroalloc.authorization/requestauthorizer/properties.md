# RequestAuthorizer Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md)
- Kind: `Properties`
- Members: `1`

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once.

<a id="caller"></a>

## Caller

- Name: `Caller`
- Modifiers: `get`
- Returns: `ISecurityContext`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L44)

The caller of this call; anonymous when the application registered no `Authorization.ISecurityContext`.

```csharp
public ISecurityContext Caller { get; }
```
