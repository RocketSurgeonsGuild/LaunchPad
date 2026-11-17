# Rocket.Surgery.LaunchPad.AspNetCore.Mvc

- Package: `Rocket.Surgery.LaunchPad.AspNetCore.Mvc`
- Version: `2.0.1-pr.1226`
- Target framework: `net11.0`
- NuGet: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.AspNetCore.Mvc)

## Namespaces

### Rocket.Surgery.LaunchPad.AspNetCore.Mvc

- [AspNetCoreMvcOptions](/LaunchPad/pr-preview/pr-3276/api/rocket.surgery.launchpad.aspnetcore.mvc/aspnetcoremvcoptions.md) -- `class` -- Options for launchpad and aspnet core.
- [RocketSurgeryMvcCoreExtensions](/LaunchPad/pr-preview/pr-3276/api/rocket.surgery.launchpad.aspnetcore.mvc/rocketsurgerymvccoreextensions.md) -- `class` `static` -- Helper methods for getting an mvc builder if you're not sure that mvc has been added to the application yet.

### Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints

- [MvcJoint](/LaunchPad/pr-preview/pr-3276/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcjoint.md) -- `class` -- Registers MVC Core, the MVC API explorer and the application parts for every assembly that depends on Rocket.Surgery.LaunchPad.AspNetCore. MVC is not native AOT compatible, which is why this joint lives in Rocket.Surgery.LaunchPad.AspNetCore.Mvc. Implements the `Joints.IServiceJoint`
- [MvcProblemDetailsJoint](/LaunchPad/pr-preview/pr-3276/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcproblemdetailsjoint.md) -- `class` -- Fills in the title and type of problem details from the client error mapping of MVC's `Mvc.ApiBehaviorOptions`, and turns off MVC's automatic 400 for an invalid model state (LaunchPad validates requests itself). Implements the `Joints.IServiceJoint`
- [MvcSerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3276/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcserilogrequestloggingjoint.md) -- `class` -- Adds the MVC and Razor Pages filters that add the action and the page handler to the request's log event, in the diagnostic context `Joints.SerilogRequestLoggingJoint` registers. Implements the `Joints.IServiceJoint`
- [MvcSystemJsonTextJoint](/LaunchPad/pr-preview/pr-3276/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcsystemjsontextjoint.md) -- `class` -- Applies the existing System.Text.Json options to MVC's JSON options, as `Joints.SystemJsonTextJoint` does to the HTTP JSON options. Implements the `Joints.IServiceJoint`
