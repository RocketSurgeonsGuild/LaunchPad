# ServiceCollectionHelper Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.Grpc](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.fluentvalidation.grpc.md)
- Type: [ServiceCollectionHelper](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.fluentvalidation.grpc/servicecollectionhelper.md)
- Kind: `Methods`
- Members: `1`

Service collection helpers with grpc

<a id="addgrpcvalidation"></a>
<a id="addgrpcvalidation-iservicecollection"></a>

## AddGrpcValidation(IServiceCollection)

- Name: `AddGrpcValidation(IServiceCollection)`
- Modifiers: `extension`
- Returns: `IServiceCollection`

Add default component for validating grpc messages

```csharp
public static class ServiceCollectionHelper
{
    public static IServiceCollection AddGrpcValidation(
        this IServiceCollection services)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceCollection`)
  service collection

## Returns

`IServiceCollection` -- service collection
