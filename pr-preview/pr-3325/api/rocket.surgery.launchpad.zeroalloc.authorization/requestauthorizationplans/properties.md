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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L145)

Every plan of the application.

```csharp
public IReadOnlyCollection<RequestAuthorizationPlan> All { get; }
```
