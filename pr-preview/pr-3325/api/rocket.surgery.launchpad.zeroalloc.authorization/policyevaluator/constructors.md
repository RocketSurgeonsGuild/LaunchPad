# PolicyEvaluator Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyEvaluator](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator.md)
- Kind: `Constructors`
- Members: `1`

Runs one policy for a caller, with the services of the call.

<a id="constructor"></a>
<a id="constructor-object-nint"></a>

## PolicyEvaluator(object, nint)

- Name: `Constructor(object, nint)`

```csharp
public delegate PolicyEvaluator
{
    public PolicyEvaluator(
        object @object,
        nint method)
    {
        // ...
    }
}
```

## Parameters

- `object` (`object`)
- `method` (`nint`)
