# IValidatorErrorMessageHandler

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.FluentValidation.Grpc](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation.grpc.md)
- Version: `2.0.1-pr.1197`
- Namespace: `Rocket.Surgery.LaunchPad.Grpc.Validation`
- Target framework: `net10.0`

A grpc error message handler

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Grpc.Validation;

public interface IValidatorErrorMessageHandler
{
    // ...
}
```

## Methods

- [Handle(IEnumerable<ValidationFailure>)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation.grpc/ivalidatorerrormessagehandler/methods.md#handle-ienumerable-validationfailure) : `string` `abstract` -- Handle sync failures
- [HandleAsync(IEnumerable<ValidationFailure>)](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.fluentvalidation.grpc/ivalidatorerrormessagehandler/methods.md#handleasync-ienumerable-validationfailure) : `Task<string>` `abstract` -- Handle async failures
