# AuthorizationResponse<TResponse>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1286`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

How a denial becomes a response: the conversions of `AuthorizationResponse`1.Unauthorized` and `AuthorizationResponse`1.Forbidden` to the response union, registered by generated code.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public static class AuthorizationResponse<TResponse>
{
    // ...
}
```

## Type Parameters

- `TResponse` -- The response union.

## Properties

- [Forbidden](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1/properties.md#forbidden) : `Func<Forbidden, TResponse>` `static` `get` -- Converts `AuthorizationResponse`1.Forbidden` to the response.
- [Unauthorized](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1/properties.md#unauthorized) : `Func<Unauthorized, TResponse>` `static` `get` -- Converts `AuthorizationResponse`1.Unauthorized` to the response.

## Methods

- [Register(Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1/methods.md#register-func-unauthorized-tresponse-func-forbidden-tresponse) `static` -- Registers the conversions. Called by generated code.
