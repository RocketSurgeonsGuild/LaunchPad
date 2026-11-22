# AuthorizationResponse<TResponse> Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationResponse<TResponse>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1.md)
- Kind: `Constructors`
- Members: `1`

How a denial becomes a response: the conversions of `AuthorizationResponse`1.Unauthorized` and `AuthorizationResponse`1.Forbidden` to the response union. LaunchPad's generator registers one for the response of every request with a policy ( [AuthorizationServiceCollectionExtensions.AddAuthorizationResponse(IServiceCollection, Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addauthorizationresponse-iservicecollection-func-unauthorized-tresponse-func-forbidden-tresponse)).

<a id="constructor"></a>
<a id="constructor-func-unauthorized-tresponse-func-forbidden-tresponse"></a>

## AuthorizationResponse(Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)

- Name: `Constructor(Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L161-L167)

How a denial becomes a response: the conversions of `AuthorizationResponse`1.Unauthorized` and `AuthorizationResponse`1.Forbidden` to the response union. LaunchPad's generator registers one for the response of every request with a policy ( [AuthorizationServiceCollectionExtensions.AddAuthorizationResponse(IServiceCollection, Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addauthorizationresponse-iservicecollection-func-unauthorized-tresponse-func-forbidden-tresponse)).

```csharp
public sealed class AuthorizationResponse<TResponse>
{
    public AuthorizationResponse(
        Func<Unauthorized, TResponse> unauthorized,
        Func<Forbidden, TResponse> forbidden)
    {
        // ...
    }
}
```

## Parameters

- `unauthorized` (`Func<Unauthorized, TResponse>`)
  Converts `Foundation.Unauthorized` to the response.
- `forbidden` (`Func<Forbidden, TResponse>`)
  Converts `Foundation.Forbidden` to the response.
