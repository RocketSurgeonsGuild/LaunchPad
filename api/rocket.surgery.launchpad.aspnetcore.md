# Rocket.Surgery.LaunchPad.AspNetCore

- Package: `Rocket.Surgery.LaunchPad.AspNetCore`
- Version: `2.0.1-beta.417`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.main.Sha.082a7c7f7334298a4eab1a2bfcbe43171fb91433/src/AspNetCore)
- NuGet: [Rocket.Surgery.LaunchPad.AspNetCore](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.AspNetCore)

## Namespaces

### Rocket.Surgery.LaunchPad.AspNetCore

- [LaunchPadHelpers](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadhelpers.md) -- `class` `static` -- Helpers for during application startup and in middleware
- [LaunchPadRequestLogging](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging.md) -- `class` `static` -- The level and the properties LaunchPad gives the request log event that Serilog's request logging writes.
- [LaunchPadRequestLoggingExtensions](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions.md) -- `class` `static` -- Adds Serilog's request logging with LaunchPad's defaults.

### Rocket.Surgery.LaunchPad.AspNetCore.Joints

- [OpenApiJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/openapijoint.md) -- `class` -- Registers OpenAPI document generation, with the LaunchPad schema and operation transformers. Implements the `Joints.IServiceJoint`
- [ProblemDetailsJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint.md) -- `class` -- Registers problem details. Rocket.Surgery.LaunchPad.AspNetCore.Mvc also fills in their title and type from MVC's client error mapping. Implements the `Joints.IServiceJoint`
- [SerilogRequestLoggingJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint.md) -- `class` -- Wires Serilog's request logging into ASP.NET Core: the `IDiagnosticContext` that `UseLaunchPadRequestLogging` collects the request's properties in. Rocket.Surgery.LaunchPad.AspNetCore.Mvc adds the MVC and Razor Pages filters that add the action and the page handler. Implements the `Joints.IServiceJoint`
- [SystemJsonTextJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint.md) -- `class` -- Applies the existing System.Text.Json options to the HTTP JSON options, which Minimal APIs, problem details and OpenAPI use. Rocket.Surgery.LaunchPad.AspNetCore.Mvc applies them to MVC's JSON options too. Implements the `Joints.IServiceJoint`
