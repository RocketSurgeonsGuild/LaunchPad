# AuthorizationResponse<TResponse>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1298`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

How a denial becomes a response: the conversions of `AuthorizationResponse`1.Unauthorized` and `AuthorizationResponse`1.Forbidden` to the response union. LaunchPad's generator registers one for the response of every request with a policy ( [AuthorizationServiceCollectionExtensions.AddAuthorizationResponse(IServiceCollection, Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addauthorizationresponse-iservicecollection-func-unauthorized-tresponse-func-forbidden-tresponse)).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class AuthorizationResponse<TResponse>
{
    // ...
}
```

## Type Parameters

- `TResponse` -- The response union.

## Constructors

- [AuthorizationResponse(Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1/constructors.md#constructor-func-unauthorized-tresponse-func-forbidden-tresponse) -- How a denial becomes a response: the conversions of `AuthorizationResponse`1.Unauthorized` and `AuthorizationResponse`1.Forbidden` to the response union. LaunchPad's generator registers one for the response of every request with a policy ( [AuthorizationServiceCollectionExtensions.AddAuthorizationResponse(IServiceCollection, Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addauthorizationresponse-iservicecollection-func-unauthorized-tresponse-func-forbidden-tresponse)).

## Properties

- [Forbidden](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1/properties.md#forbidden) : `Func<Forbidden, TResponse>` `get` -- Converts `Foundation.Forbidden` to the response.
- [Unauthorized](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1/properties.md#unauthorized) : `Func<Unauthorized, TResponse>` `get` -- Converts `Foundation.Unauthorized` to the response.
