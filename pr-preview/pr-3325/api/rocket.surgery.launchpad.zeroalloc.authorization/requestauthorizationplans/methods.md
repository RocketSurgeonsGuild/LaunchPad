# RequestAuthorizationPlans Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlans](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans.md)
- Kind: `Methods`
- Members: `2`

The plans of the application, by request, and the policies they name, by the name an endpoint carries. One for the application.

<a id="find"></a>
<a id="find-type"></a>

## Find(Type)

- Name: `Find(Type)`
- Modifiers: `nullable`
- Returns: [RequestAuthorizationPlan?](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L161-L162)

The plan of a request, if it has policies.

```csharp
public sealed class RequestAuthorizationPlans
{
    public RequestAuthorizationPlan? Find(
        Type requestType)
    {
        // ...
    }
}
```

## Parameters

- `requestType` (`Type`)
  The request.

<a id="findpolicy"></a>
<a id="findpolicy-string"></a>

## FindPolicy(string?)

- Name: `FindPolicy(string?)`
- Modifiers: `nullable`
- Returns: [PolicyRequirement?](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L171-L177)

A policy some request names, by its [PolicyRequirement.Key](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#key): the policy's name, with the arguments the request passes it. Null for a name no request names, which is then a policy of the application's own.

```csharp
public sealed class RequestAuthorizationPlans
{
    public PolicyRequirement? FindPolicy(
        string? name)
    {
        // ...
    }
}
```

## Parameters

- `name` (`string?`)
  The policy name on an endpoint.
