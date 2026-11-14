# Rocket.Surgery.LaunchPad.AspNetCore

- Package: `Rocket.Surgery.LaunchPad.AspNetCore`
- Version: `2.0.1-pr.1191`
- Target framework: `net10.0`
- NuGet: [Rocket.Surgery.LaunchPad.AspNetCore](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.AspNetCore)

## Namespaces

### Rocket.Surgery.LaunchPad.AspNetCore

- [AspNetCoreOptions](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/aspnetcoreoptions.md) -- `class` -- Options for launchpad and aspnet core.
- [LaunchPadHelpers](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/launchpadhelpers.md) -- `class` `static` -- Helpers for during application startup and in middleware
- [LaunchPadRequestLogging](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestlogging.md) -- `class` `static` -- The level and the properties LaunchPad gives the request log event that Serilog's request logging writes.
- [LaunchPadRequestLoggingExtensions](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/launchpadrequestloggingextensions.md) -- `class` `static` -- Adds Serilog's request logging with LaunchPad's defaults.
- [RocketSurgeryMvcCoreExtensions](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/rocketsurgerymvccoreextensions.md) -- `class` `static` -- Helper methods for getting an mvc builder if you're not sure that mvc has been added to the application yet.

### Rocket.Surgery.LaunchPad.AspNetCore.Filters

- [ProblemDetailsExceptionFilter<T>](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/problemdetailsexceptionfilter-1.md) -- `class` `abstract` -- An exception filter that catches the given problem details exception and uses the given status code to return the result

### Rocket.Surgery.LaunchPad.AspNetCore.Joints

- [AspNetCoreJoint](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/aspnetcorejoint.md) -- `class` -- Registers MVC, the API explorer and the application parts for every assembly that depends on this one. The other ASP.NET Core joints run after this one. Implements the `Joints.IServiceJoint`
- [OpenApiJoint](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/openapijoint.md) -- `class` -- Registers OpenAPI document generation, with the LaunchPad schema and operation transformers. Implements the `Joints.IServiceJoint`
- [ProblemDetailsJoint](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint.md) -- `class` -- Registers problem details and the exception handler status codes for the LaunchPad exceptions. Implements the `Joints.IServiceJoint`
- [SerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint.md) -- `class` -- Wires Serilog's request logging into ASP.NET Core: the MVC and Razor Pages filters that add the action and the page handler to the request's log event, and the `IDiagnosticContext` they and `UseLaunchPadRequestLogging` collect the properties in. Implements the `Joints.IServiceJoint`
- [SystemJsonTextJoint](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.aspnetcore/systemjsontextjoint.md) -- `class` -- Applies the existing System.Text.Json options to the MVC and the HTTP JSON options. Implements the `Joints.IServiceJoint`
