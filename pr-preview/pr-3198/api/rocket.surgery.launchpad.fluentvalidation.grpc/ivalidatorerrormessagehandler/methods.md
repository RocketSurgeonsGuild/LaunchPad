# IValidatorErrorMessageHandler Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.Grpc](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation.grpc.md)
- Type: [IValidatorErrorMessageHandler](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation.grpc/ivalidatorerrormessagehandler.md)
- Kind: `Methods`
- Members: `2`

A grpc error message handler

<a id="handle"></a>
<a id="handle-ienumerable-validationfailure"></a>

## Handle(IEnumerable<ValidationFailure>)

- Name: `Handle(IEnumerable<ValidationFailure>)`
- Modifiers: `abstract`
- Returns: `string`

Handle sync failures

```csharp
public interface IValidatorErrorMessageHandler
{
    public abstract string Handle(
        IEnumerable<ValidationFailure> failures)
    {
        // ...
    }
}
```

## Parameters

- `failures` (`IEnumerable<ValidationFailure>`)

<a id="handleasync"></a>
<a id="handleasync-ienumerable-validationfailure"></a>

## HandleAsync(IEnumerable<ValidationFailure>)

- Name: `HandleAsync(IEnumerable<ValidationFailure>)`
- Modifiers: `abstract`
- Returns: `Task<string>`

Handle async failures

```csharp
public interface IValidatorErrorMessageHandler
{
    public abstract Task<string> HandleAsync(
        IEnumerable<ValidationFailure> failures)
    {
        // ...
    }
}
```

## Parameters

- `failures` (`IEnumerable<ValidationFailure>`)
