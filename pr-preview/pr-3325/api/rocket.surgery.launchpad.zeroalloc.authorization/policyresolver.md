# PolicyResolver

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1286`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/PolicyResolver.cs)

Resolves a policy class for a call. Called by generated code.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public static class PolicyResolver
{
    // ...
}
```

## Methods

- [Resolve(IServiceProvider)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyresolver/methods.md#resolve-iserviceprovider) : `TPolicy` `static` -- The policy from the services of the call when it is registered (as `AddZeroAllocAuthorization()` does), otherwise a new one with its dependencies from them.
