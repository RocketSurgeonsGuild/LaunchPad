# LaunchPad never serializes JSON through reflection

LaunchPad reads and writes JSON only with the JSON contexts it is given: the application's, registered explicitly with `AddJsonTypeInfoResolver`, and LaunchPad's own, chained behind them. The `JsonSerializerOptions` LaunchPad builds never get the reflection based resolver, so a type no JSON context knows fails the same way in a JIT application as in a native AOT one, and the gap shows up in development rather than after publishing. Enums are written as camel case strings by generic converters that a generator registers for the enums the JSON contexts reach, since the reflection based enum converter needs dynamic code.

## Considered Options

- **Keep the reflection resolver last in JIT applications.** Rejected: an application would work under JIT and fail only once published trimmed or AOT, when it is most expensive to find out.
- **Discover JSON contexts automatically** (Specular scanning or a generated Joint). Deferred: creating the contexts without reflection and coupling Specular to System.Text.Json cost more than one registration line.

## Consequences

ASP.NET Core's own HTTP and MVC JSON options keep the reflection resolver ASP.NET adds to a JIT application; LaunchPad chains its resolvers in front of it but does not remove it. An application that wants reflection in LaunchPad's options adds the resolver itself, and LaunchPad leaves it in place. An enum no JSON context reaches has no converter, so where reflection is available it is written as a number. The NodaTime and spatial packages, which do not reference Primitives, chain their own converter-backed resolvers.
