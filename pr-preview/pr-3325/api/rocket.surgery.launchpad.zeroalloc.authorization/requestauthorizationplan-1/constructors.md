# RequestAuthorizationPlan<TRequest> Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlan<TRequest>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1.md)
- Kind: `Constructors`
- Members: `1`

The plan of `TRequest`.

<a id="constructor"></a>
<a id="constructor-policygroup"></a>

## RequestAuthorizationPlan(PolicyGroup[])

- Name: `Constructor(PolicyGroup[])`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L65-L70)

The plan of `TRequest`.

```csharp
public sealed class RequestAuthorizationPlan<TRequest>
{
    public RequestAuthorizationPlan(
        params PolicyGroup[] groups)
    {
        // ...
    }
}
```

## Parameters

- `groups` (`PolicyGroup[]`)
  The requirements, all of which must pass.
