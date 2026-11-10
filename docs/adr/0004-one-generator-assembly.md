# LaunchPad's source generators are one analyzer assembly shipped in Foundation

The API generators (operation hosts) and the generators that used to ship with Foundation (property tracking, `InheritFrom`, `Mutable`) are one analyzer assembly, `Rocket.Surgery.LaunchPad.Analyzers`, packed into `Rocket.Surgery.LaunchPad.Foundation`. The `Rocket.Surgery.LaunchPad.Api` package is gone. We merged them so that mediator detection lives in one place: generating the handler of a patch operation needs the same mediator profile as the operation hosts, and LaunchPad itself depends on no mediator, so the only shared place for that knowledge is the generator assembly.

## Considered Options

- **Keep `Rocket.Surgery.LaunchPad.Api` and generate patch handlers there.** Rejected: property tracking, which defines a patch operation, would live in one package and its handler in another, and an application using patch operations without operation hosts would need the API package anyway.
- **Generate patch handlers in Foundation's analyzer with its own copy of the mediator detection.** Rejected: two copies of the mediator profiles that every new mediator would have to keep in step.

## Consequences

Every project that references Foundation runs the operation host generators. They only write code for types that declare `[Operation<TRequest>]`, so a project without hosts pays only for the generators being loaded. This amends the 2026-11-08 amendment of ADR 0002: the API generators are still one assembly, but no longer a package of their own.
