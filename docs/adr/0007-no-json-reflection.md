# LaunchPad never serializes JSON through reflection

LaunchPad reads and writes JSON only with the JSON contexts it is given: the application's, registered explicitly with `AddJsonTypeInfoResolver`, and LaunchPad's own, chained behind them. The `JsonSerializerOptions` LaunchPad builds never get the reflection based resolver, so a type no JSON context knows fails the same way in a JIT application as in a native AOT one, and the gap shows up in development rather than after publishing. Enums are written as camel case strings by generic converters that a generator registers for the enums the JSON contexts reach, since the reflection based enum converter needs dynamic code.

An executable gets its JSON context written by its build: every library lists its JSON types in an assembly-level manifest, and an MSBuild task reads the manifests of the referenced assemblies and writes a `JsonSerializerContext`, with a Joint that registers it, as source before the compiler runs. It is written by MSBuild rather than by a generator because the System.Text.Json source generator never sees another generator's output, and it names types from referenced assemblies because their metadata includes the members LaunchPad's own generators (property tracking, `[InheritFrom]`) add to them.

## Considered Options

- **Keep the reflection resolver last in JIT applications.** Rejected: an application would work under JIT and fail only once published trimmed or AOT, when it is most expensive to find out.
- **Scan referenced assemblies for mediator requests at build time** instead of reading manifests. Rejected: the task would need every mediator library's rules; the generator that writes the manifest already has them, shared with the API generators.
- **Discover hand-written JSON contexts automatically** (Specular scanning or a generated Joint). Deferred: creating the contexts without reflection and coupling Specular to System.Text.Json cost more than one registration line.

## Consequences

ASP.NET Core's own HTTP and MVC JSON options keep the reflection resolver ASP.NET adds to a JIT application; LaunchPad chains its resolvers in front of it but does not remove it. An application that wants reflection in LaunchPad's options adds the resolver itself, and LaunchPad leaves it in place. An enum no JSON context reaches has no converter, so where reflection is available it is written as a number. The NodaTime and spatial packages, which do not reference Primitives, chain their own converter-backed resolvers. The JSON generators ship in Primitives' analyzer package with the union generator, an exception to [ADR 0004](0004-one-generator-assembly.md) recorded there.
