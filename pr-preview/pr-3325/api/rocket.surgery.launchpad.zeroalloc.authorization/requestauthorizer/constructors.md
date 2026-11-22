# RequestAuthorizer Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md)
- Kind: `Constructors`
- Members: `1`

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once.

<a id="constructor"></a>
<a id="constructor-iserviceprovider-authorizationoutcomes-requestauthorizationplans-ioptions-requestauthorizationoptions"></a>

## RequestAuthorizer(IServiceProvider, AuthorizationOutcomes, RequestAuthorizationPlans, IOptions<RequestAuthorizationOptions>)

- Name: `Constructor(IServiceProvider, AuthorizationOutcomes, RequestAuthorizationPlans, IOptions<RequestAuthorizationOptions>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L34-L39)

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once.

```csharp
public sealed class RequestAuthorizer
{
    public RequestAuthorizer(
        IServiceProvider services,
        AuthorizationOutcomes outcomes,
        RequestAuthorizationPlans plans,
        IOptions<RequestAuthorizationOptions> options)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceProvider`)
  The services of the call, from which the caller and the policies are resolved.
- `outcomes` ([AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md))
  What each policy decided for this call.
- `plans` ([RequestAuthorizationPlans](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans.md))
  The plans of the application.
- `options` (`IOptions<RequestAuthorizationOptions>`)
  How the caller's scopes are read.
