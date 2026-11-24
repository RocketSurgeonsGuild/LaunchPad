# ScopeRequirements Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [ScopeRequirements](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirements.md)
- Kind: `Methods`
- Members: `2`

Checks the scopes of a caller: what the `EvaluateAsync` LaunchPad writes for a policy with scope attributes calls. A policy is checked wherever its request is dispatched, with or without a transport, so the check is the policy's own.

<a id="evaluate"></a>
<a id="evaluate-isecuritycontext-scoperequirement"></a>

## Evaluate(ISecurityContext, ScopeRequirement[])

- Name: `Evaluate(ISecurityContext, ScopeRequirement[])`
- Modifiers: `static`
- Returns: `UnitResult<AuthorizationFailure>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/ScopeRequirements.cs#L53-L60)

Whether the caller satisfies every requirement; otherwise a failure with [ScopeRequirements.MissingScopeCode](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirements/fields.md#missingscopecode) that names the scopes of the first requirement the caller doesn't satisfy.

```csharp
public static class ScopeRequirements
{
    public static UnitResult<AuthorizationFailure> Evaluate(
        ISecurityContext context,
        params ScopeRequirement[] requirements)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ISecurityContext`)
  The caller.
- `requirements` (`ScopeRequirement[]`)
  The scope attributes of the policy.

<a id="granted"></a>
<a id="granted-isecuritycontext"></a>

## Granted(ISecurityContext)

- Name: `Granted(ISecurityContext)`
- Modifiers: `static`
- Returns: `IReadOnlySet<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/ScopeRequirements.cs#L39-L42)

The scopes the caller has.

```csharp
public static class ScopeRequirements
{
    public static IReadOnlySet<string> Granted(
        ISecurityContext context)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ISecurityContext`)
  The caller.
