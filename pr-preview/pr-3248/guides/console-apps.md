# Console applications

`Rocket.Surgery.LaunchPad.Sdk.Console` is the [LaunchPad SDK](../../concepts/launchpad-sdk/) for command line applications. It
adds the same LaunchPad packages as `Rocket.Surgery.LaunchPad.Sdk`, and chains Dovetail's console SDK,
`Joinery.Dovetail.Sdk.Console`, instead of `Joinery.Dovetail.Sdk`. The commands run on Cysharp's
[ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework), and Dovetail writes the entry point.

In the snippets, replace `1.0.0` with the version of the SDK you use.

## The project

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk.Console/1.0.0">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
```

There is no `OutputType` to set and no `Program.cs` to write. The SDK:

- makes the project an `Exe`, which Dovetail finds a generic host (`HostApplication`) for;
- adds ConsoleAppFramework (a source generator, so nothing ships with the application) and `Microsoft.Extensions.Hosting`;
- adds `Joinery.Dovetail.Hosting`, which writes `Program.cs` to `obj/`, and `Rocket.Surgery.LaunchPad.Hosting`, whose
  [templates](../../concepts/templates/) bring the health check, logging and telemetry Joints when their libraries are referenced;
- adds the [integration packages](../choosing-packages/) of what the project references, exactly as the LaunchPad SDK does.

To apply it to every project of a repository, use `<Project Sdk="Rocket.Surgery.LaunchPad.Sdk.Console">` in a
`Directory.Build.props` and pin the version in `global.json` under `msbuild-sdks`, as for the LaunchPad SDK. Only use one of the two
LaunchPad SDKs in a project.

The generated `Program` creates the `HostApplicationBuilder`, configures it with every [Joint](../../concepts/joints/) the
application imports, builds and starts the host, applies the `ConsoleApp` Joints, runs the command the command line names and stops
the host. The exit code is ConsoleAppFramework's.

## Commands

A command is a method marked `[Command]` on a class that ConsoleAppFramework creates from the host's services, so it can take
anything another Joint registered. An `IConsoleAppJoint` registers the classes:

```csharp
using ConsoleAppFramework;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.Extensions.DependencyInjection;

public sealed class Greeter
{
    public string Greet(string name) => $"Hello, {name}";
}

internal sealed class GreetCommands(Greeter greeter)
{
    /// <summary>Greets someone.</summary>
    /// <param name="name">Who to greet.</param>
    [Command("greet")]
    public void Greet(string name = "world") => Console.WriteLine(greeter.Greet(name));
}

[DovetailExport]
internal sealed class AppJoint : IServiceJoint, IConsoleAppJoint
{
    public void Register(IDovetailContext context, IServiceCollection services) => services.AddSingleton<Greeter>();

    public void Register(IDovetailContext context, ConsoleApp.ConsoleAppBuilder app) => app.Add<GreetCommands>();
}
```

```bash
dotnet run -- greet --name LaunchPad
```

`[Command("")]` makes a method the root command, which runs when no command is given. ConsoleAppFramework's types are internal,
so the command classes and the `ConsoleApp` Joints are too.

:::note
ConsoleAppFramework reads the `Add` calls in the application's own compilation. The Joints that add commands have to be in the
application's assembly: a library, LaunchPad's included, cannot contribute commands. Libraries still contribute services,
configuration and everything else through their Joints.
:::

## Writing the entry point yourself

Set `DovetailUseDefaultProgram` to `false` to keep the generated `Program.cs` out, for example so tests can run the application
in-process with their own output and configuration. The `ConsoleApp` Joint is still declared, so the entry point applies it with
`ApplyConsoleApp`:

```csharp
var builder = Host.CreateApplicationBuilder(args);

IDovetailContext? dovetail = null;
using var host = await builder.ConfigureDovetail(
    context => context.ConfigureHostCreated<IHost>((created, _) => dovetail = created)
);

ConsoleApp.ServiceProvider = host.Services;
var app = ConsoleApp.Create();
await app.ApplyConsoleApp(dovetail!);

await host.StartAsync();
try
{
    await app.RunAsync(args, startHost: false, stopHost: false, disposeServiceProvider: false);
}
finally
{
    await host.StopAsync();
}

return Environment.ExitCode;
```

ConsoleAppFramework keeps the service provider and the exit code in statics (`ConsoleApp.ServiceProvider`,
`Environment.ExitCode`), so tests that run the application in-process must not run in parallel. The
[`Sample.Command`](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/main/sample/Sample.Command) sample does this, and also
configures its DryIoc container from an `IDryIocJoint`.

## Properties

| Property                             | Default                     | Effect                                                                   |
| ------------------------------------ | --------------------------- | ------------------------------------------------------------------------ |
| `DovetailConsoleApp`                 | `true` with the console SDK | Declares the `ConsoleApp` Joint and writes `Program.cs`.                 |
| `DovetailUseDefaultProgram`          | `true`                      | `false` keeps the generated `Program.cs` out.                            |
| `DovetailConsoleAppFrameworkVersion` | the version Dovetail pins   | The ConsoleAppFramework version the SDK adds.                            |
| `LaunchPadDovetailSdkVersion`        | the version the SDK pins    | The version of `Joinery.Dovetail.Sdk.Console` the SDK chains.            |
| `LaunchPadEnable<Tech>`              | `true`                      | `false` opts the project out of a technology, as with the LaunchPad SDK. |