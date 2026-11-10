# LaunchPad SDK

The **LaunchPad SDK**, `Rocket.Surgery.LaunchPad.Sdk`, is an MSBuild SDK an application opts into so that the right LaunchPad
packages are chosen from what the project references. It is the only LaunchPad entry the application needs: no
`PackageReference` to LaunchPad or Dovetail.

## How it builds on Dovetail's SDK

The LaunchPad SDK chains `Joinery.Dovetail.Sdk`, so there is no second SDK entry. When a project builds:

1. Dovetail's SDK decides the project's [host model](../host-models/) and adds `Joinery.Dovetail.Hosting`, plus
   `Joinery.Dovetail.Serilog` and `Joinery.Dovetail.OpenTelemetry` when the project references Serilog or OpenTelemetry. LaunchPad
   turns Serilog on by default.
2. The LaunchPad SDK reads that host model and the project's [direct references](../integration-packages/#direct-references), and
   adds the [integration packages](../integration-packages/#the-packages-the-sdk-chooses) that match.
3. NuGet restores everything both SDKs added, so the packages behave like ordinary references. A package the project already
   references itself is left alone.

All of it happens while MSBuild evaluates the project, before restore. Nothing is decided at run time.

## Opting in

Name the SDK next to the one the project already uses:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <Sdk Name="Rocket.Surgery.LaunchPad.Sdk" Version="1.0.0" />
</Project>
```

or make it the project's SDK (it brings `Microsoft.NET.Sdk` through Dovetail's):

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk/1.0.0">
```

or apply it to every project in a `Directory.Build.props`, with the version pinned in `global.json`:

```xml
<Project Sdk="Rocket.Surgery.LaunchPad.Sdk">
```

```json
{
    "msbuild-sdks": {
        "Rocket.Surgery.LaunchPad.Sdk": "1.0.0"
    }
}
```

A file-based app uses the directive form, `#:sdk Rocket.Surgery.LaunchPad.Sdk@1.0.0`, and gets what a console application needs
without a `#:package` directive for LaunchPad.

[Getting started](../../guides/) walks through each of these.

## Choosing another Dovetail version

The SDK chains the Dovetail SDK version it was built with. To chain another, set `LaunchPadDovetailSdkVersion` before the SDK is
imported: as a global property, or in a `Directory.Build.props` that sets it and then imports the SDK with
`<Import Project="Sdk.props" Sdk="Rocket.Surgery.LaunchPad.Sdk" />`.