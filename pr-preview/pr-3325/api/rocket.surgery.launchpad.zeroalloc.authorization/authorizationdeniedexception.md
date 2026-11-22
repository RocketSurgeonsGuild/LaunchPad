# AuthorizationDeniedException

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1296`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/AuthorizationDeniedException.cs)
- Inherits: `Exception`

A request was denied where there is no response to answer with: a stream request, or a response that has no case for the denial. It carries the `Foundation.Unauthorized` or `Foundation.Forbidden` problem, which each transport writes as that case.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class AuthorizationDeniedException
    : System.Exception
{
    // ...
}
```

## Constructors

- [AuthorizationDeniedException(Problem)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdeniedexception/constructors.md#constructor-problem) -- A request was denied where there is no response to answer with: a stream request, or a response that has no case for the denial. It carries the `Foundation.Unauthorized` or `Foundation.Forbidden` problem, which each transport writes as that case.

## Properties

- [Problem](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdeniedexception/properties.md#problem) : `Problem` `get` -- The `Foundation.Unauthorized` or `Foundation.Forbidden` problem.
