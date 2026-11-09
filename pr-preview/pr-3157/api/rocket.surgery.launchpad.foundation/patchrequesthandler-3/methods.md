# PatchRequestHandler<TRequest, TPatch, TResponse> Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.foundation.md)
- Type: [PatchRequestHandler<TRequest, TPatch, TResponse>](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.foundation/patchrequesthandler-3.md)
- Kind: `Methods`
- Members: `1`

A common handler for creating patch methods

<a id="handle"></a>
<a id="handle-tpatch-cancellationtoken"></a>

## Handle(TPatch, CancellationToken)

- Name: `Handle(TPatch, CancellationToken)`
- Modifiers: `virtual`
- Returns: `Task<TResponse>`

Handles a request

```csharp
public abstract class PatchRequestHandler<TRequest, TPatch, TResponse>
{
    public virtual Task<TResponse> PatchRequestHandler<TRequest, TPatch, Handle(TPatch request, CancellationToken cancellationToken)
    {
        // ...
    }
}
```

## Parameters

- `request` (`TPatch`)
  The request
- `cancellationToken` (`CancellationToken`)
  Cancellation token

## Returns

`Task<TResponse>` -- Response from the request
