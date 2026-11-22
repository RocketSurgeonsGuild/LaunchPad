# RequestAuthorizationPlan Fields

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlan](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Kind: `Fields`
- Members: `2`

The policies a request requires. LaunchPad's generator writes one for every request with a policy attribute, and registers it with the services of the application ( [AuthorizationServiceCollectionExtensions.AddRequestAuthorizationPlan(IServiceCollection, PolicyGroup[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addrequestauthorizationplan-iservicecollection-policygroup)).

<a id="maxalternatives"></a>

## MaxAlternatives

- Name: `MaxAlternatives`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

The most alternatives that are published.

```csharp
public const static int MaxAlternatives
```

<a id="policyprefix"></a>

## PolicyPrefix

- Name: `PolicyPrefix`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

What an edge policy name starts with; the rest is the plan's [RequestAuthorizationPlan.Id](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#id).

```csharp
public const static string PolicyPrefix
```
