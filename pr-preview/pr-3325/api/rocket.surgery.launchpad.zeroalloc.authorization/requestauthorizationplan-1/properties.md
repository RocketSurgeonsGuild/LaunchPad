# RequestAuthorizationPlan<TRequest> Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlan<TRequest>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1.md)
- Kind: `Properties`
- Members: `1`

The plan of `TRequest`.

<a id="instance"></a>

## Instance

- Name: `Instance`
- Modifiers: `static` `nullable` `get`
- Returns: [RequestAuthorizationPlan<TRequest>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1.md)

The plan, or null for a request without policies.

```csharp
public static RequestAuthorizationPlan<TRequest>? Instance { get; }
```
