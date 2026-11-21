# SystemJsonTextJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1270`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3317-merge.Sha.1621484e9867a9384634a5b693a135763b433b4a/src/AspNetCore/Joints/SystemJsonTextJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Applies the existing System.Text.Json options to the HTTP JSON options, which Minimal APIs, problem details and OpenAPI use. Rocket.Surgery.LaunchPad.AspNetCore.Mvc applies them to MVC's JSON options too. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class SystemJsonTextJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

ASP.NET Core puts a reflection based resolver in the HTTP JSON options of a JIT application. This joint takes it out, so a type no JSON context knows fails the same way under JIT as under native AOT (ADR 0007). A reflection resolver the application adds itself stays. An application that wants ASP.NET Core's resolver back sets the MSBuild property `LaunchPadAspNetCoreJsonReflection` to `true`, or the [SystemJsonTextJoint.KeepReflectionSwitch](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint/fields.md#keepreflectionswitch) switch with `AppContext.SetSwitch` before the host is built.

## Constructors

- [SystemJsonTextJoint](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## Fields

- [KeepReflectionSwitch](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint/fields.md#keepreflectionswitch) : `string` `static` -- The `AppContext` switch that keeps ASP.NET Core's reflection resolver in the HTTP JSON options.

## See Also

- `Joints.IServiceJoint`
