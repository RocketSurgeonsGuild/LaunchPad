# RequestAuthorizationPlans Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlans](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans.md)
- Kind: `Properties`
- Members: `1`

The plans of the application, by request, and the policies they name, by the name an endpoint carries. One for the application.

<a id="all"></a>

## All

- Name: `All`
- Modifiers: `get`
- Returns: [IReadOnlyCollection<RequestAuthorizationPlan>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L152)

Every plan of the application.

```csharp
public IReadOnlyCollection<RequestAuthorizationPlan> All { get; }
```
