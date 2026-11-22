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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/AuthorizationDeniedException.cs#L9-L12)

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
