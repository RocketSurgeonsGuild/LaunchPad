# Getting Started

An application uses LaunchPad through the [LaunchPad SDK](../concepts/launchpad-sdk/). It names the SDK once, and the SDK adds
the LaunchPad and Dovetail packages that match the project: its [host model](../concepts/host-models/) and the libraries it
references directly.

In the snippets, replace `1.0.0` with the version of `Rocket.Surgery.LaunchPad.Sdk` you use.

## A worker or console application

Add the SDK next to the one the project already names:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <Sdk Name="Rocket.Surgery.LaunchPad.Sdk" Version="1.0.0" />
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.14" />
  </ItemGroup>
</Project>
```

and build the host with `ConfigureDovetail()`, which applies every [Joint](../concepts/joints/) the application imports:

```csharp
using var host = await Host.CreateApplicationBuilder(args).ConfigureDovetail();
await host.RunAsync();
```

`ConfigureDovetail()` is generated into the project's root namespace, so add a `using` for it if `Program.cs` is in another one.

The SDK finds a generic host (`HostApplication`) and adds `Joinery.Dovetail.Hosting`, which generates `ConfigureDovetail()`, and
`Rocket.Surgery.LaunchPad.Hosting`, whose [templates](../concepts/templates/) bring health checks, logging and telemetry Joints
when their libraries are referenced.

## An ASP.NET Core application

The same line in a `Microsoft.NET.Sdk.Web` project gives the `WebApplication` host model, which also gets
`Rocket.Surgery.LaunchPad.AspNetCore`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <Sdk Name="Rocket.Surgery.LaunchPad.Sdk" Version="1.0.0" />
</Project>
```

```csharp
var app = await WebApplication.CreateBuilder(args).ConfigureDovetail();
app.MapHealthChecks("/health");
await app.RunAsync();
```

ASP.NET Core has the health checks in its shared framework, so `LaunchPadHealthChecksJoint` registers them without another
reference.

## Every project in a repository

Apply the SDK once in `Directory.Build.props`:

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk">
</Project>
```

and pin its version in `global.json`:

```json
{
    "msbuild-sdks": {
        "Rocket.Surgery.LaunchPad.Sdk": "1.0.0"
    }
}
```

Libraries get nothing from the SDK until they reference a technology LaunchPad has a package for: a library has no host model.

## A file-based app

A single `.cs` file run with `dotnet run app.cs` names the SDK with a directive and needs no `#:package` for LaunchPad:

```csharp
#:sdk Rocket.Surgery.LaunchPad.Sdk@1.0.0
#:property RootNamespace=MyApp

using MyApp;

using var host = await Host.CreateApplicationBuilder(args).ConfigureDovetail();
await host.RunAsync();
```

It gets the generic host, `Rocket.Surgery.LaunchPad.Foundation`, `Hosting` and `NodaTime`, and Dovetail's Serilog and
OpenTelemetry packages. `RootNamespace` names the namespace `ConfigureDovetail()` is generated in; a file-based app would
otherwise take it from the file name.

## Next

- [Choosing integration packages](./choosing-packages/): what the SDK adds for each technology, and how to turn one off.
- [Writing a Joint](./writing-a-joint/): add wiring of your own.
- The [samples](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/main/sample/Sample.Sdk) consume LaunchPad only through the
  SDK, the way an application outside the repository does.