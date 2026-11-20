# SystemJsonTextJoint Fields

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [SystemJsonTextJoint](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint.md)
- Kind: `Fields`
- Members: `1`

Applies the existing System.Text.Json options to the HTTP JSON options, which Minimal APIs, problem details and OpenAPI use. Rocket.Surgery.LaunchPad.AspNetCore.Mvc applies them to MVC's JSON options too. Implements the `Joints.IServiceJoint`

<a id="keepreflectionswitch"></a>

## KeepReflectionSwitch

- Name: `KeepReflectionSwitch`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3291-merge.Sha.13e74febbd2cc111b9b844ce764f2522bbd95912/src/AspNetCore/Joints/SystemJsonTextJoint.cs)

The `AppContext` switch that keeps ASP.NET Core's reflection resolver in the HTTP JSON options.

```csharp
public const static string KeepReflectionSwitch
```
