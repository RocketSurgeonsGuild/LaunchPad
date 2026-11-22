# RequestAuthorizationPlan Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlan](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Kind: `Properties`
- Members: `5`

The policies a request requires. LaunchPad's generator writes one for every request with a policy attribute, and registers it with the services of the application ( [AuthorizationServiceCollectionExtensions.AddRequestAuthorizationPlan(IServiceCollection, PolicyGroup[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addrequestauthorizationplan-iservicecollection-policygroup)).

<a id="groups"></a>

## Groups

- Name: `Groups`
- Modifiers: `get`
- Returns: [IReadOnlyList<PolicyGroup>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policygroup.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L82)

The requirements, all of which must pass.

```csharp
public IReadOnlyList<PolicyGroup> Groups { get; }
```

<a id="id"></a>

## Id

- Name: `Id`
- Modifiers: `get`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L79)

What names the plan at the edge: the request's full type name.

```csharp
public string Id { get; }
```

<a id="policyname"></a>

## PolicyName

- Name: `PolicyName`
- Modifiers: `get`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L85)

The name of the edge policy that checks this plan.

```csharp
public string PolicyName { get; }
```

<a id="requesttype"></a>

## RequestType

- Name: `RequestType`
- Modifiers: `get`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L76)

The request.

```csharp
public Type RequestType { get; }
```

<a id="scopealternatives"></a>

## ScopeAlternatives

- Name: `ScopeAlternatives`
- Modifiers: `get`
- Returns: `IReadOnlyList<IReadOnlyList<string>>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L91)

The scopes that satisfy the plan, as alternatives: any one set is enough. A stacked policy adds its scopes to every alternative, a group of alternatives multiplies them; duplicates and alternatives that contain another are dropped.

```csharp
public IReadOnlyList<IReadOnlyList<string>> ScopeAlternatives { get; }
```
