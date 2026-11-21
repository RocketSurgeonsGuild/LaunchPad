# AuthorizationResponse<TResponse> Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationResponse<TResponse>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1.md)
- Kind: `Methods`
- Members: `1`

How a denial becomes a response: the conversions of `AuthorizationResponse`1.Unauthorized` and `AuthorizationResponse`1.Forbidden` to the response union, registered by generated code.

<a id="register"></a>
<a id="register-func-unauthorized-tresponse-func-forbidden-tresponse"></a>

## Register(Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)

- Name: `Register(Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L216-L218)

Registers the conversions. Called by generated code.

```csharp
public static class AuthorizationResponse<TResponse>
{
    public static void Register(
        Func<Unauthorized, TResponse> unauthorized,
        Func<Forbidden, TResponse> forbidden)
    {
        // ...
    }
}
```

## Parameters

- `unauthorized` (`Func<Unauthorized, TResponse>`)
  Converts `AuthorizationResponse`1.Unauthorized` to the response.
- `forbidden` (`Func<Forbidden, TResponse>`)
  Converts `AuthorizationResponse`1.Forbidden` to the response.
