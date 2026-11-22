# AuthorizationProblems Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [AuthorizationProblems](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/authorizationproblems.md)
- Kind: `Methods`
- Members: `3`

How an `Foundation.Unauthorized` or `Foundation.Forbidden` problem is written over HTTP: as the generated servers write the case.

<a id="challenge"></a>
<a id="challenge-authorizationdecision-string"></a>

## Challenge(AuthorizationDecision, string?)

- Name: `Challenge(AuthorizationDecision, string?)`
- Modifiers: `static` `nullable`
- Returns: `string?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.AspNetCore/Authorization/AuthorizationProblems.cs#L53-L65)

The `WWW-Authenticate` challenge of a denial (RFC 6750): the scopes to ask for, and `insufficient_scope` when the caller is known and lacks one. Null when the denial names no scopes.

```csharp
public static class AuthorizationProblems
{
    public static string? Challenge(
        AuthorizationDecision decision,
        string? existing = null)
    {
        // ...
    }
}
```

## Parameters

- `decision` (`AuthorizationDecision`)
  The denial.
- `existing` (`string?`) `optional`
  The challenge the authentication scheme already wrote, which is kept.

<a id="details"></a>
<a id="details-problem"></a>

## Details(Problem)

- Name: `Details(Problem)`
- Modifiers: `static`
- Returns: `ProblemDetails`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.AspNetCore/Authorization/AuthorizationProblems.cs#L30-L42)

The problem as the problem details a union case answers with: the case's name in `case`, and its data.

```csharp
public static class AuthorizationProblems
{
    public static ProblemDetails Details(
        Problem problem)
    {
        // ...
    }
}
```

## Parameters

- `problem` (`Problem`)
  The problem.

<a id="statuscode"></a>
<a id="statuscode-problem"></a>

## StatusCode(Problem)

- Name: `StatusCode(Problem)`
- Modifiers: `static`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.AspNetCore/Authorization/AuthorizationProblems.cs#L24)

The status of the problem: 401 for `Foundation.Unauthorized`, otherwise 403.

```csharp
public static class AuthorizationProblems
{
    public static int StatusCode(
        Problem problem)
    {
        // ...
    }
}
```

## Parameters

- `problem` (`Problem`)
  The problem.
