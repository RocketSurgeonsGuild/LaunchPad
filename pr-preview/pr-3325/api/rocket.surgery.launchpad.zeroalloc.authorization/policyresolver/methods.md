# PolicyResolver Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyResolver](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyresolver.md)
- Kind: `Methods`
- Members: `1`

Resolves a policy class for a call. Called by generated code.

<a id="resolve"></a>
<a id="resolve-iserviceprovider"></a>

## Resolve(IServiceProvider)

- Name: `Resolve(IServiceProvider)`
- Modifiers: `static`
- Returns: `TPolicy`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/PolicyResolver.cs#L17)

The policy from the services of the call when it is registered (as `AddZeroAllocAuthorization()` does), otherwise a new one with its dependencies from them.

```csharp
public static class PolicyResolver
{
    public static TPolicy Resolve<TPolicy>(
        IServiceProvider services)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceProvider`)
  The services of the call.
