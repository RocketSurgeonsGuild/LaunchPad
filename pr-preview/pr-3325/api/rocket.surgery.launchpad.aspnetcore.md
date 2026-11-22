# Rocket.Surgery.LaunchPad.AspNetCore

- Package: `Rocket.Surgery.LaunchPad.AspNetCore`
- Version: `2.0.1-pr.1296`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/AspNetCore)
- NuGet: [Rocket.Surgery.LaunchPad.AspNetCore](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.AspNetCore)

## Namespaces

### Rocket.Surgery.LaunchPad.AspNetCore

- [LaunchPadHealthCheckExtensions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckextensions.md) -- `class` `static` -- Maps LaunchPad's health check endpoints.
- [LaunchPadRequestLogging](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging.md) -- `class` `static` -- The level and the properties LaunchPad gives the request log event that Serilog's request logging writes.
- [LaunchPadRequestLoggingExtensions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions.md) -- `class` `static` -- Adds Serilog's request logging with LaunchPad's defaults.

### Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks

- [LaunchPadHealthCheckOptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions.md) -- `class` `sealed` -- Where LaunchPad maps the health check endpoints ( `MapLaunchPadHealthChecks`), and on which port.
- [LaunchPadHealthCheckResponseWriter](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckresponsewriter.md) -- `class` `static` -- Writes a `HealthChecks.HealthReport` as JSON in the shape the HealthChecks.UI dashboard reads ( `UIResponseWriter`).

### Rocket.Surgery.LaunchPad.AspNetCore.Joints

- [HealthChecksJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/healthchecksjoint.md) -- `class` -- Registers the health check services and the options of LaunchPad's health check endpoints ( `MapLaunchPadHealthChecks`), bound from the [LaunchPadHealthCheckOptions.SectionName](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/fields.md#sectionname) configuration section. Implements the `Joints.IServiceJoint`
- [OpenApiJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/openapijoint.md) -- `class` -- Registers OpenAPI document generation, with the LaunchPad schema and operation transformers. Implements the `Joints.IServiceJoint`
- [ProblemDetailsJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint.md) -- `class` -- Registers problem details. Rocket.Surgery.LaunchPad.AspNetCore.Mvc also fills in their title and type from MVC's client error mapping. Implements the `Joints.IServiceJoint`
- [SerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint.md) -- `class` -- Wires Serilog's request logging into ASP.NET Core: the `IDiagnosticContext` that `UseLaunchPadRequestLogging` collects the request's properties in. Rocket.Surgery.LaunchPad.AspNetCore.Mvc adds the MVC and Razor Pages filters that add the action and the page handler. Implements the `Joints.IServiceJoint`
- [SystemJsonTextJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint.md) -- `class` -- Applies the existing System.Text.Json options to the HTTP JSON options, which Minimal APIs, problem details and OpenAPI use. Rocket.Surgery.LaunchPad.AspNetCore.Mvc applies them to MVC's JSON options too. Implements the `Joints.IServiceJoint`
