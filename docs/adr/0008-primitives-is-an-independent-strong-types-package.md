# Primitives is an independent strong types package

`Rocket.Surgery.LaunchPad.Primitives` holds LaunchPad's strong types and nothing else, and depends on no other LaunchPad package. Strong types are built on [SuperStrong](https://superstrong.dev) (`SuperStrong.Types`): Primitives ships ready-made strong types (email addresses, country and currency codes, IBANs, ages, amounts and so on, each a rules set and a type built on it, with the `DateRange`, `TimeRange` and `Money` records beside them), and the Joints that describe strong types to OpenAPI, HotChocolate and EF Core as templates written into the application. An application or library opts in by referencing Primitives; nothing else brings it.

Everything Primitives used to hold that isn't a strong type moved to Foundation: the standard error cases (`Problem`, `NotFound`, `Invalid` and the rest), the exceptions and `IProblemDetailsData`, the union generator, the JSON context runtime with its generators and build task, and the default Joints (options and logging, service discovery, resilience, System.Text.Json). They are what every LaunchPad application is built on, so they belong with Foundation; the namespaces changed from `Rocket.Surgery.LaunchPad.Primitives` to `Rocket.Surgery.LaunchPad.Foundation`, with no forwarding.

## Considered Options

- **EricksonLopez.DomainPrimitives** (tried first, #3166). Rejected: its 3.0.0 packages ship its source generator where NuGet doesn't run it and an OpenAPI package with no generator, and its OpenAPI support is a Swashbuckle filter.
- **SuperStrong's own integration packages** for OpenAPI, EF Core and HotChocolate. Rejected: they cap their dependencies below the versions LaunchPad builds against on .NET 11, the OpenAPI one leaves out the rules (lengths, patterns, ranges), and they find strong types by reflection. LaunchPad's templates read the strong types from a source generator instead.
- **A new package beside Primitives** for strong types, leaving Primitives as it was. Rejected: Primitives would stay a dependency of every application for types that belong to Foundation, and the name fits strong types better than error cases.
- **Integration packages** (one per technology, [ADR 0002](0002-package-per-integration.md)) instead of templates. Rejected: the Joints are a few lines each against libraries the application already references, and as templates Primitives depends on none of them.

## Consequences

Moving the types is a breaking change for every application (namespaces, and Primitives no longer arrives through other packages). The API generators match the error cases by their Foundation names. Primitives is the one place LaunchPad depends on a strong types library, so replacing SuperStrong touches only Primitives. Primitives' Joints read a registry of the application's strong types written by Primitives' own source generator, in its own analyzer package (`Rocket.Surgery.LaunchPad.Primitives.Analyzers`): an exception to [ADR 0004](0004-one-generator-assembly.md), since Primitives can't depend on the analyzer assembly Foundation carries.
