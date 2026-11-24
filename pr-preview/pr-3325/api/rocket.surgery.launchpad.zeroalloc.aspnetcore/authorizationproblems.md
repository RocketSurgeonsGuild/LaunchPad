# AuthorizationProblems

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.AspNetCore/Authorization/AuthorizationProblems.cs)

How an `Foundation.Unauthorized` or `Foundation.Forbidden` problem is written over HTTP: as the generated servers write the case.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

public static class AuthorizationProblems
{
    // ...
}
```

## Methods

- [Challenge(AuthorizationDecision, string?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/authorizationproblems/methods.md#challenge-authorizationdecision-string) : `string?` `static` -- The `WWW-Authenticate` challenge of a denial (RFC 6750): the scopes to ask for, and `insufficient_scope` when the caller is known and lacks one. Null when the denial names no scopes.
- [Details(Problem)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/authorizationproblems/methods.md#details-problem) : `ProblemDetails` `static` -- The problem as the problem details a union case answers with: the case's name in `case`, and its data.
- [StatusCode(Problem)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/authorizationproblems/methods.md#statuscode-problem) : `int` `static` -- The status of the problem: 401 for `Foundation.Unauthorized`, otherwise 403.
