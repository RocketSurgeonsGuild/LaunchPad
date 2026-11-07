# RestfulApiOptions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-beta.314`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net10.0`

Options used for Restful Api Options.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore;

public class RestfulApiOptions
{
    // ...
}
```

## Constructors

- [RestfulApiOptions](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/restfulapioptions/constructors.md#constructor)

## Properties

- [Builders](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/restfulapioptions/properties.md#builders) : `Collection<RestfulApiMethodBuilder>` `get` -- The related method builders that are used to identify methods by name, prefix, type, etc.
- [MethodStatusCodeMap](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/restfulapioptions/properties.md#methodstatuscodemap) : `IDictionary<RestfulApiMethod, int>` `get` -- The cache of default status codes for a given method type.
- [ValidationProblemDetailsType](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/restfulapioptions/properties.md#validationproblemdetailstype) : `Type` `get; set` -- The response type documented for the [RestfulApiOptions.ValidationStatusCode](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/restfulapioptions/properties.md#validationstatuscode). A validation integration (such as Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore) replaces it with its own problem details.
- [ValidationStatusCode](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/restfulapioptions/properties.md#validationstatuscode) : `int` `get; set` -- The factory to use for Validation results
