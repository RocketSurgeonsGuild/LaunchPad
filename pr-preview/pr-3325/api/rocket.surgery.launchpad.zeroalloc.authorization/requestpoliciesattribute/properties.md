# RequestPoliciesAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestPoliciesAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestpoliciesattribute.md)
- Kind: `Properties`
- Members: `1`

On an endpoint: the request it dispatches, whose plan states the scopes the endpoint requires. The generated HTTP servers put it on an operation whose request has policies; the policies themselves are on the endpoint by name.

<a id="requesttype"></a>

## RequestType

- Name: `RequestType`
- Modifiers: `get`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L127)

The request.

```csharp
public Type RequestType { get; }
```
