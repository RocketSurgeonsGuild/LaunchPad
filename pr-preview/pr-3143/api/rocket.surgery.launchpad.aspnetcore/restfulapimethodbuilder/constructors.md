# RestfulApiMethodBuilder Constructors

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md)
- Kind: `Constructors`
- Members: `1`

This class allows you to define methods default conventions when using the [RestfulApiController](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.aspnetcore/restfulapicontroller.md) This allows the response codes to be automatically inferred based on the rules defined.

<a id="constructor"></a>
<a id="constructor-restfulapimethod"></a>

## RestfulApiMethodBuilder(RestfulApiMethod)

- Name: `Constructor(RestfulApiMethod)`

Create a method build for the given [RestfulApiMethod](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.aspnetcore/restfulapimethod.md).

```csharp
public class RestfulApiMethodBuilder
{
    public RestfulApiMethodBuilder(
        RestfulApiMethod method)
    {
        // ...
    }
}
```

## Parameters

- `method` ([RestfulApiMethod](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.aspnetcore/restfulapimethod.md))
