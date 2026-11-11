# LaunchPad's source generators are one analyzer assembly shipped in Foundation

The API generators (operation hosts) and the generators that used to ship with Foundation (property tracking, `InheritFrom`, `Mutable`) are one analyzer assembly, `Rocket.Surgery.LaunchPad.Analyzers`, packed into `Rocket.Surgery.LaunchPad.Foundation`. The `Rocket.Surgery.LaunchPad.Api` package is gone. We merged them so that mediator detection lives in one place: generating the handler of a patch operation needs the same mediator profile as the operation hosts, and LaunchPad itself depends on no mediator, so the only shared place for that knowledge is the generator assembly.

## Considered Options

- **Keep `Rocket.Surgery.LaunchPad.Api` and generate patch handlers there.** Rejected: property tracking, which defines a patch operation, would live in one package and its handler in another, and an application using patch operations without operation hosts would need the API package anyway.
- **Generate patch handlers in Foundation's analyzer with its own copy of the mediator detection.** Rejected: two copies of the mediator profiles that every new mediator would have to keep in step.

## Consequences

Every project that references Foundation runs the operation host generators. They only write code for types that declare `[Operation<TRequest>]`, so a project without hosts pays only for the generators being loaded. This amends the 2026-11-08 amendment of ADR 0002: the API generators are still one assembly, but no longer a package of their own.

Amended 2026-11-10 (#3152), and since reverted (see below): the JSON generators, which write the camel case enum converters and the JSON context manifest ([ADR 0007](0007-no-json-reflection.md)), are not in this assembly but in `Rocket.Surgery.LaunchPad.Primitives.Analyzers`, the analyzer package of `Rocket.Surgery.LaunchPad.Primitives` that also holds the union generator. Every project that references Primitives needs them, including one without Foundation. The manifest recognises mediator requests with the same table as the operation host generators: `src/Shared/MediatorLibraries.cs` is one source file compiled into both assemblies, so there is still a single description of each mediator to keep in step.

Amended again 2026-11-10 (#3166): the union generator and the JSON generators are back in this assembly. They were apart only because Primitives needed them without Foundation; Primitives is now an optional strong types package ([ADR 0008](0008-primitives-is-an-independent-strong-types-package.md)), and the standard error cases, the unions and the JSON context moved to Foundation, so there is one analyzer assembly again and `src/Shared/MediatorLibraries.cs` is compiled once.
