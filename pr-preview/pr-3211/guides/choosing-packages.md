# Choosing integration packages

With the [LaunchPad SDK](../../concepts/launchpad-sdk/) a project turns on LaunchPad support for a technology by referencing the
technology itself. There is no LaunchPad package to look up: the SDK adds the
[integration packages](../../concepts/integration-packages/) that match.

## Turning a technology on

Reference the library directly from the project that uses it:

```xml
<ItemGroup>
  <PackageReference Include="NodaTime" Version="3.3.7" />
  <PackageReference Include="HotChocolate.AspNetCore" Version="16.7.5" />
</ItemGroup>
```

This project gets `Rocket.Surgery.LaunchPad.NodaTime`, `Rocket.Surgery.LaunchPad.HotChocolate` and, because it has both,
`Rocket.Surgery.LaunchPad.NodaTime.HotChocolate`. The table of what each technology adds is in
[Integration packages](../../concepts/integration-packages/#the-packages-the-sdk-chooses).

Only [direct references](../../concepts/integration-packages/#direct-references) count. If NodaTime reaches the project only
through another library, the project gets no NodaTime package; reference NodaTime itself to get one.

## Seeing what was added

The packages the SDK adds are ordinary `PackageReference` items marked `IsImplicitlyDefined`, so they show up in
`dotnet list package` and in the IDE's dependencies like any other. A package the project references itself is left as the
project wrote it, which is also how to pin one to another version.

## Turning a technology off

Set the technology's `LaunchPadEnable<Technology>` property to `false`. That removes its own package and every integration
package it is part of:

```xml
<PropertyGroup>
  <!-- keeps NodaTime, drops Rocket.Surgery.LaunchPad.NodaTime and Rocket.Surgery.LaunchPad.NodaTime.HotChocolate -->
  <LaunchPadEnableNodaTime>false</LaunchPadEnableNodaTime>
</PropertyGroup>
```

`LaunchPadEnableHosting` and `LaunchPadEnableAspNetCore` turn off the packages that come with the host model. Dovetail's own
packages have properties of their own, such as `DovetailEnableSerilog` and `DovetailEnableOpenTelemetry`.

## Logging and telemetry sinks

Serilog sinks and enrichers and OpenTelemetry instrumentations are chosen the same way: reference the package, and the
[template](../../concepts/templates/) that turns it on is written into the application.

```xml
<ItemGroup>
  <PackageReference Include="Serilog.Sinks.Console" Version="6.1.1" />
  <PackageReference Include="Serilog.Sinks.Async" Version="2.1.0" />
  <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.20.1" />
</ItemGroup>
```

gives an ASP.NET Core application `LaunchPadSerilogConsoleJoint` and `LaunchPadAspNetCoreInstrumentationJoint`.