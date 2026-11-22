# RequestAuthorizer Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md)
- Kind: `Constructors`
- Members: `1`

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once. A policy sees the caller and nothing of the request: it is the same check wherever it runs.

<a id="constructor"></a>
<a id="constructor-iserviceprovider-authorizationoutcomes-ioptions-requestauthorizationoptions"></a>

## RequestAuthorizer(IServiceProvider, AuthorizationOutcomes, IOptions<RequestAuthorizationOptions>)

- Name: `Constructor(IServiceProvider, AuthorizationOutcomes, IOptions<RequestAuthorizationOptions>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L33-L37)

Decides a request's plan for the caller of a call, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. One for each call: the mediator pipeline and the edge both decide through it, so a policy runs once. A policy sees the caller and nothing of the request: it is the same check wherever it runs.

```csharp
public sealed class RequestAuthorizer
{
    public RequestAuthorizer(
        IServiceProvider services,
        AuthorizationOutcomes outcomes,
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
- `options` (`IOptions<RequestAuthorizationOptions>`)
  How the caller's scopes are read.
