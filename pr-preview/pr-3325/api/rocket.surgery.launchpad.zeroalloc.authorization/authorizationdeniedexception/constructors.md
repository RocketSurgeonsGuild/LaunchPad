# AuthorizationDeniedException Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationDeniedException](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdeniedexception.md)
- Kind: `Constructors`
- Members: `1`

A request was denied where there is no response to answer with: a stream request, or a response that has no case for the denial. It carries the `Foundation.Unauthorized` or `Foundation.Forbidden` problem, which each transport writes as that case.

<a id="constructor"></a>
<a id="constructor-problem"></a>

## AuthorizationDeniedException(Problem)

- Name: `Constructor(Problem)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationDeniedException.cs#L9-L12)

A request was denied where there is no response to answer with: a stream request, or a response that has no case for the denial. It carries the `Foundation.Unauthorized` or `Foundation.Forbidden` problem, which each transport writes as that case.

```csharp
public sealed class AuthorizationDeniedException
{
    public AuthorizationDeniedException(
        Problem problem)
    {
        // ...
    }
}
```

## Parameters

- `problem` (`Problem`)
  The `Foundation.Unauthorized` or `Foundation.Forbidden` problem.
