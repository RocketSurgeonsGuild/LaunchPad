# SystemJsonTextJoint Fields

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3274/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [SystemJsonTextJoint](/LaunchPad/pr-preview/pr-3274/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint.md)
- Kind: `Fields`
- Members: `1`

Applies the existing System.Text.Json options to the HTTP JSON options, which Minimal APIs, problem details and OpenAPI use. Rocket.Surgery.LaunchPad.AspNetCore.Mvc applies them to MVC's JSON options too. Implements the `Joints.IServiceJoint`

<a id="keepreflectionswitch"></a>

## KeepReflectionSwitch

- Name: `KeepReflectionSwitch`
- Modifiers: `static` `const`
- Returns: `string`

The `AppContext` switch that keeps ASP.NET Core's reflection resolver in the HTTP JSON options.

```csharp
public const static string KeepReflectionSwitch
```
