# AuthorizationResponse<TResponse> Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationResponse<TResponse>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1.md)
- Kind: `Properties`
- Members: `2`

How a denial becomes a response: the conversions of `AuthorizationResponse`1.Unauthorized` and `AuthorizationResponse`1.Forbidden` to the response union. LaunchPad's generator registers one for the response of every request with a policy ( [AuthorizationServiceCollectionExtensions.AddAuthorizationResponse(IServiceCollection, Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addauthorizationresponse-iservicecollection-func-unauthorized-tresponse-func-forbidden-tresponse)).

<a id="forbidden"></a>

## Forbidden

- Name: `Forbidden`
- Modifiers: `get`
- Returns: `Func<Forbidden, TResponse>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L139)

Converts `Foundation.Forbidden` to the response.

```csharp
public Func<Forbidden, TResponse> Forbidden { get; }
```

<a id="unauthorized"></a>

## Unauthorized

- Name: `Unauthorized`
- Modifiers: `get`
- Returns: `Func<Unauthorized, TResponse>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L136)

Converts `Foundation.Unauthorized` to the response.

```csharp
public Func<Unauthorized, TResponse> Unauthorized { get; }
```
