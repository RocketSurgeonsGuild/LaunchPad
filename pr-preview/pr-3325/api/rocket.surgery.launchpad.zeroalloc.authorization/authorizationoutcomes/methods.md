# AuthorizationOutcomes Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md)
- Kind: `Methods`
- Members: `2`

What each policy decided for the current call, by policy name and arguments. The edge and the mediator pipeline share it through the call's service scope, so a policy runs once.

<a id="record"></a>
<a id="record-policyrequirement-policyoutcome"></a>

## Record(PolicyRequirement, PolicyOutcome)

- Name: `Record(PolicyRequirement, PolicyOutcome)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs#L28-L30)

Records what a policy decided for this call.

```csharp
public sealed class AuthorizationOutcomes
{
    public void Record(
        PolicyRequirement policy,
        PolicyOutcome outcome)
    {
        // ...
    }
}
```

## Parameters

- `policy` ([PolicyRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md))
  The policy.
- `outcome` ([PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md))
  Its outcome.

<a id="tryget"></a>
<a id="tryget-policyrequirement-policyoutcome"></a>

## TryGet(PolicyRequirement, PolicyOutcome)

- Name: `TryGet(PolicyRequirement, PolicyOutcome)`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs#L19-L21)

The outcome recorded for a policy, if it has run for this call.

```csharp
public sealed class AuthorizationOutcomes
{
    public bool TryGet(
        PolicyRequirement policy,
        out PolicyOutcome outcome)
    {
        // ...
    }
}
```

## Parameters

- `policy` ([PolicyRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md))
  The policy.
- `outcome` ([PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md))
  Its outcome.
