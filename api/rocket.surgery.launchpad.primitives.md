# Rocket.Surgery.LaunchPad.Primitives

- Package: `Rocket.Surgery.LaunchPad.Primitives`
- Version: `2.0.1-beta.324`
- Target framework: `net10.0`
- NuGet: [Rocket.Surgery.LaunchPad.Primitives](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Primitives)

## Namespaces

### Rocket.Surgery.LaunchPad.Primitives

- [ExistingValueOptions](/LaunchPad/api/rocket.surgery.launchpad.primitives/existingvalueoptions.md) -- `class` `static` -- Helper methods for creating `Primitives.ExistingValueOptionsFactory`1` instances.
- [ExistingValueOptionsFactory<TOptions>](/LaunchPad/api/rocket.surgery.launchpad.primitives/existingvalueoptionsfactory-1.md) -- `class` -- Implementation of `Options.IOptionsFactory`1`.
- [IProblemDetailsData](/LaunchPad/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata.md) -- `interface` -- A common interface used for definiting problems
- [NotAuthorizedException](/LaunchPad/api/rocket.surgery.launchpad.primitives/notauthorizedexception.md) -- `class` -- NotAuthorizedException.
- [NotFoundException](/LaunchPad/api/rocket.surgery.launchpad.primitives/notfoundexception.md) -- `class` -- NotFoundException.
- [ProblemDetailsException](/LaunchPad/api/rocket.surgery.launchpad.primitives/problemdetailsexception.md) -- `class` `abstract` -- NotFoundException.
- [RequestFailedException](/LaunchPad/api/rocket.surgery.launchpad.primitives/requestfailedexception.md) -- `class` -- An exception for failed requests, with overloads to take additional details

### Rocket.Surgery.LaunchPad.Primitives.Extensions

- [UsefulEnumerableExtensions](/LaunchPad/api/rocket.surgery.launchpad.primitives/usefulenumerableextensions.md) -- `class` `static` -- Useful extensions for enumerations.

### Rocket.Surgery.LaunchPad.Primitives.Joints

- [DefaultJoint](/LaunchPad/api/rocket.surgery.launchpad.primitives/defaultjoint.md) -- `class` -- The joint every LaunchPad application starts from. Implements the `Joints.IServiceJoint`
- [ServiceDiscoveryCoreJoint](/LaunchPad/api/rocket.surgery.launchpad.primitives/servicediscoverycorejoint.md) -- `class` -- A joint that registers service discovery and adds it to every `Http.HttpClient` the application creates.
- [StandardResilienceJoint](/LaunchPad/api/rocket.surgery.launchpad.primitives/standardresiliencejoint.md) -- `class` -- A joint that adds the standard resilience handler to every `Http.HttpClient` the application creates.
- [SystemTextJsonJoint](/LaunchPad/api/rocket.surgery.launchpad.primitives/systemtextjsonjoint.md) -- `class` -- A joint that configures `Json.JsonSerializerOptions` the LaunchPad way: camel case names and camel case enums. Implements the `Joints.IServiceJoint`
