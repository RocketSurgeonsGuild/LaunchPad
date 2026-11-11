# LaunchPadJsonServiceCollectionExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.foundation.md)
- Type: [LaunchPadJsonServiceCollectionExtensions](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.foundation/launchpadjsonservicecollectionextensions.md)
- Kind: `Methods`
- Members: `1`

Registers JSON type info resolvers for the `SystemTextJsonJoint` to chain into the application's `Json.JsonSerializerOptions`.

<a id="addjsontypeinforesolver"></a>
<a id="addjsontypeinforesolver-iservicecollection-ijsontypeinforesolver-int"></a>

## AddJsonTypeInfoResolver(IServiceCollection, IJsonTypeInfoResolver, int)

- Name: `AddJsonTypeInfoResolver(IServiceCollection, IJsonTypeInfoResolver, int)`
- Modifiers: `extension`
- Returns: `IServiceCollection`

Adds a JSON type info resolver, usually a source generated `Serialization.JsonSerializerContext`, to the front of the `JsonSerializerOptions.TypeInfoResolverChain` of the application's `Json.JsonSerializerOptions`, and of the ASP.NET Core HTTP and MVC JSON options when LaunchPad's ASP.NET Core package is used.

```csharp
public static class LaunchPadJsonServiceCollectionExtensions
{
    public static IServiceCollection AddJsonTypeInfoResolver(
        this IServiceCollection services,
        IJsonTypeInfoResolver resolver,
        int order = 0)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceCollection`)
  The service collection.
- `resolver` (`IJsonTypeInfoResolver`)
  The resolver.
- `order` (`int`) `optional`
  Where the resolver goes in the chain: lower comes first. See [JsonTypeInfoResolverRegistration.LibraryOrder](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration/fields.md#libraryorder).

## Returns

`IServiceCollection` -- The service collection.

## Remarks

This is how a native AOT or trimmed application gives LaunchPad the metadata for its own types: `services.AddJsonTypeInfoResolver(MyJsonContext.Default)`, in a Joint or in the host's service configuration. Adding the same resolver twice chains it once.
