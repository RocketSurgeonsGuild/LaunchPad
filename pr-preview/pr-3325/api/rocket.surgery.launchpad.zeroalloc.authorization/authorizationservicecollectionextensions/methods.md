# AuthorizationServiceCollectionExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationServiceCollectionExtensions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions.md)
- Kind: `Methods`
- Members: `3`

Registers what authorizes requests by their policies. LaunchPad's generator writes the calls for the requests of each project, into the `AddLaunchPadRequestAuthorization()` its joint applies; an application without Dovetail calls that method itself.

<a id="addauthorizationresponse"></a>
<a id="addauthorizationresponse-iservicecollection-func-unauthorized-tresponse-func-forbidden-tresponse"></a>

## AddAuthorizationResponse(IServiceCollection, Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)

- Name: `AddAuthorizationResponse(IServiceCollection, Func<Unauthorized, TResponse>, Func<Forbidden, TResponse>)`
- Modifiers: `extension`
- Returns: `IServiceCollection`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationServiceCollectionExtensions.cs#L55-L57)

Registers how a denial becomes a response, unless the response has it registered.

```csharp
public static class AuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorizationResponse<TResponse>(
        this IServiceCollection services,
        Func<Unauthorized, TResponse> unauthorized,
        Func<Forbidden, TResponse> forbidden)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceCollection`)
  The services.
- `unauthorized` (`Func<Unauthorized, TResponse>`)
  Converts `Foundation.Unauthorized` to the response.
- `forbidden` (`Func<Forbidden, TResponse>`)
  Converts `Foundation.Forbidden` to the response.

<a id="addlaunchpadauthorization"></a>
<a id="addlaunchpadauthorization-iservicecollection"></a>

## AddLaunchPadAuthorization(IServiceCollection)

- Name: `AddLaunchPadAuthorization(IServiceCollection)`
- Modifiers: `extension`
- Returns: `IServiceCollection`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationServiceCollectionExtensions.cs#L20-L24)

Registers what every plan needs: the [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md) and the [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) of each call, and the [RequestAuthorizationPlans](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans.md) of the application.

```csharp
public static class AuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddLaunchPadAuthorization(
        this IServiceCollection services)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceCollection`)
  The services.

<a id="addrequestauthorizationplan"></a>
<a id="addrequestauthorizationplan-iservicecollection-policygroup"></a>

## AddRequestAuthorizationPlan(IServiceCollection, PolicyGroup[])

- Name: `AddRequestAuthorizationPlan(IServiceCollection, PolicyGroup[])`
- Modifiers: `extension`
- Returns: `IServiceCollection`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationServiceCollectionExtensions.cs#L33-L41)

Registers the plan of a request, unless it has one.

```csharp
public static class AuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddRequestAuthorizationPlan<TRequest>(
        this IServiceCollection services,
        params PolicyGroup[] groups)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceCollection`)
  The services.
- `groups` (`PolicyGroup[]`)
  The requirements, all of which must pass.
