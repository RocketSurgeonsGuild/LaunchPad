# AuthorizationResponse<TResponse> Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationResponse<TResponse>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1.md)
- Kind: `Properties`
- Members: `2`

How a denial becomes a response: the conversions of `AuthorizationResponse`1.Unauthorized` and `AuthorizationResponse`1.Forbidden` to the response union, registered by generated code.

<a id="forbidden"></a>

## Forbidden

- Name: `Forbidden`
- Modifiers: `static` `nullable` `get`
- Returns: `Func<Forbidden, TResponse>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

Converts `AuthorizationResponse`1.Forbidden` to the response.

```csharp
public static Func<Forbidden, TResponse>? Forbidden { get; }
```

<a id="unauthorized"></a>

## Unauthorized

- Name: `Unauthorized`
- Modifiers: `static` `nullable` `get`
- Returns: `Func<Unauthorized, TResponse>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

Converts `AuthorizationResponse`1.Unauthorized` to the response.

```csharp
public static Func<Unauthorized, TResponse>? Unauthorized { get; }
```
