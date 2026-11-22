# RequestAuthorizationPlans Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlans](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans.md)
- Kind: `Methods`
- Members: `2`

The plans of the application, by the name the edge knows them by, and what the edge has learned about its policies. One for the application.

<a id="find"></a>
<a id="find-string"></a>

## Find(string)

- Name: `Find(string)`
- Modifiers: `nullable`
- Returns: [RequestAuthorizationPlan?](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L187-L188)

The plan of the request with this id, if it has policies.

```csharp
public sealed class RequestAuthorizationPlans
{
    public RequestAuthorizationPlan? Find(
        string id)
    {
        // ...
    }
}
```

## Parameters

- `id` (`string`)
  The request's full type name.

<a id="findbypolicyname"></a>
<a id="findbypolicyname-string"></a>

## FindByPolicyName(string?)

- Name: `FindByPolicyName(string?)`
- Modifiers: `nullable`
- Returns: [RequestAuthorizationPlan?](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L194-L196)

The plan an edge policy name stands for; null for any other policy name, and for a request without a plan.

```csharp
public sealed class RequestAuthorizationPlans
{
    public RequestAuthorizationPlan? FindByPolicyName(
        string? policyName)
    {
        // ...
    }
}
```

## Parameters

- `policyName` (`string?`)
  The policy name on an endpoint.
