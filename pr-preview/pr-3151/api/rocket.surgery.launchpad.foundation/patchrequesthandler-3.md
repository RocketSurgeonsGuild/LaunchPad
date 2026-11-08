# PatchRequestHandler<TRequest, TPatch, TResponse>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1180`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Implements: `IRequestHandler<TPatch, TResponse>`

A common handler for creating patch methods

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public abstract class PatchRequestHandler<TRequest, TPatch, TResponse>
    : MediatR.IRequestHandler<TPatch, TResponse>
    where TRequest : IRequest<TResponse>
    where TPatch : IPropertyTracking<TRequest>, IRequest<TResponse>
{
    // ...
}
```

## Type Parameters

- `TPatch` -- The patch object itself
- `TRequest` -- The request type that will be run after the patch has been applied
- `TResponse` -- The final result object

## Remarks

The based handler using Mediator

## Methods

- [Handle(TPatch, CancellationToken)](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.foundation/patchrequesthandler-3/methods.md#handle-tpatch-cancellationtoken) : `Task<TResponse>` `virtual` -- Handles a request
