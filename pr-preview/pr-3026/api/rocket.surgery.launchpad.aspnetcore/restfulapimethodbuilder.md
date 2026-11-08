# RestfulApiMethodBuilder

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1152`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Composition`
- Target framework: `net10.0`
- Implements: `IRestfulApiMethodMatcher`

This class allows you to define methods default conventions when using the [RestfulApiController](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapicontroller.md) This allows the response codes to be automatically inferred based on the rules defined.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Composition;

public class RestfulApiMethodBuilder
    : Rocket.Surgery.LaunchPad.AspNetCore.Composition.IRestfulApiMethodMatcher
{
    // ...
}
```

## Constructors

- [RestfulApiMethodBuilder(RestfulApiMethod)](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/constructors.md#constructor-restfulapimethod) -- Create a method build for the given [RestfulApiMethod](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethod.md).

## Methods

- [HasParameter(Index)](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/methods.md#hasparameter-index) : [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md) -- Match only if a parameter exists at the given index
- [MatchName(string, string[])](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/methods.md#matchname-string-string) : [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md) -- Match based one of the given method names
- [MatchParameterCount(int)](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/methods.md#matchparametercount-int) : [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md) -- Match only if there are a given number of parameters
- [MatchParameterName(Index, string, string[])](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/methods.md#matchparametername-index-string-string) : [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md) -- Matched on the names of the parameter at the given index
- [MatchParameterPrefix(Index, string, string[])](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/methods.md#matchparameterprefix-index-string-string) : [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md) -- Match against the prefixes for the given parameter at the given index
- [MatchParameterSuffix(Index, string, string[])](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/methods.md#matchparametersuffix-index-string-string) : [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md) -- Match against the suffixes for the given parameter at the given index
- [MatchParameterType(Index, Type)](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/methods.md#matchparametertype-index-type) : [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md) -- Match the type parameter at the given index.
- [MatchPrefix(string, string[])](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/methods.md#matchprefix-string-string) : [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md) -- Match against one of the given prefixes
- [MatchSuffix(string, string[])](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder/methods.md#matchsuffix-string-string) : [RestfulApiMethodBuilder](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.aspnetcore/restfulapimethodbuilder.md) -- Match against one of the given suffixes
