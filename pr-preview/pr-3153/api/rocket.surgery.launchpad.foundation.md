# Rocket.Surgery.LaunchPad.Foundation

- Package: `Rocket.Surgery.LaunchPad.Foundation`
- Version: `2.0.1-pr.1157`
- Target framework: `net10.0`
- NuGet: [Rocket.Surgery.LaunchPad.Foundation](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Foundation)

## Namespaces

### Rocket.Surgery.LaunchPad.Foundation

- [Assigned<T>](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/assigned-1.md) -- `class` -- The Assigned type is used to differentiate between not set and set input values.
- [ExcludeFromGenerationAttribute](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/excludefromgenerationattribute.md) -- `class` `sealed` -- Exclude the given property from the generation via launch pad source generators
- [FoundationOptions](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/foundationoptions.md) -- `class` -- Common foundation options
- [GenerationIgnoreAttribute](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/generationignoreattribute.md) -- `class` `sealed` -- Exclude the given property from the generation via launch pad source generators
- [InheritFromAttribute](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/inheritfromattribute.md) -- `class` `sealed` -- Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.
- [InheritFromAttribute<T>](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1.md) -- `class` `sealed` -- Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.
- [IPropertyTracking](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/ipropertytracking.md) -- `interface` -- Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties
- [IPropertyTracking<T>](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/ipropertytracking-1.md) -- `interface` -- Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties
- [MutableAttribute](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/mutableattribute.md) -- `class` `sealed` -- Creates a new class that is a mutable version of the given class.
- [PatchRequestHandler<TRequest, TPatch, TResponse>](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/patchrequesthandler-3.md) -- `class` `abstract` -- A common handler for creating patch methods

### Rocket.Surgery.LaunchPad.Foundation.Joints

- [MediatRJoint](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.foundation/mediatrjoint.md) -- `class` -- A joint that registers MediatR and every handler found in the assemblies that depend on it. Implements the `Joints.IServiceJoint`
