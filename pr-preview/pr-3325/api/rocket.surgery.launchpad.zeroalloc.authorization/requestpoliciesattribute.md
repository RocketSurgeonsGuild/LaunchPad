# RequestPoliciesAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)
- Inherits: `Attribute`

On an endpoint: the request it dispatches, whose plan states the scopes the endpoint requires. The generated HTTP servers put it on an operation whose request has policies; the policies themselves are on the endpoint by name.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class RequestPoliciesAttribute
    : System.Attribute
{
    // ...
}
```

## Constructors

- [RequestPoliciesAttribute(Type)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestpoliciesattribute/constructors.md#constructor-type) -- On an endpoint: the request it dispatches, whose plan states the scopes the endpoint requires. The generated HTTP servers put it on an operation whose request has policies; the policies themselves are on the endpoint by name.

## Properties

- [RequestType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestpoliciesattribute/properties.md#requesttype) : `Type` `get` -- The request.
