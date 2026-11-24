# PolicyEvaluator

- Kind: `delegate`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/PolicyEvaluator.cs)
- Inherits: `MulticastDelegate`

Runs one policy for a caller, with the services of the call.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed delegate System.Threading.Tasks.ValueTask<ZeroAlloc.Results.UnitResult<ZeroAlloc.Authorization.AuthorizationFailure>> PolicyEvaluator(
    System.IServiceProvider services,
    ZeroAlloc.Authorization.ISecurityContext context,
    System.Threading.CancellationToken cancellationToken)
```

## Delegate Parameters

- `services` (`IServiceProvider`) -- The services of the call, from which the policy is resolved: the plan's registration registers it.
- `context` (`ISecurityContext`) -- The caller.
- `cancellationToken` (`CancellationToken`) -- The cancellation token of the call.

## Constructors

- [PolicyEvaluator(object, nint)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator/constructors.md#constructor-object-nint)

## Methods

- [BeginInvoke(IServiceProvider, ISecurityContext, CancellationToken, AsyncCallback, object)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator/methods.md#begininvoke-iserviceprovider-isecuritycontext-cancellationtoken-asynccallback-object) : `IAsyncResult` `virtual`
- [EndInvoke(IAsyncResult)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator/methods.md#endinvoke-iasyncresult) : `ValueTask<UnitResult<AuthorizationFailure>>` `virtual`
