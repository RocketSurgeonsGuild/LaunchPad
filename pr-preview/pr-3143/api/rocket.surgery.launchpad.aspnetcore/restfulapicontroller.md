# RestfulApiController

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3143/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1157`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore`
- Target framework: `net10.0`
- Inherits: `ControllerBase`

The base controller that opts a controller into the restful api conventions, which document its response types from the name and shape of each action.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore;

public abstract class RestfulApiController
    : Microsoft.AspNetCore.Mvc.ControllerBase
{
    // ...
}
```
