# Union results are C# 15 unions, with a portable form until .NET 10 is dropped

An operation that can fail responds with a union result instead of throwing, and every LaunchPad union is a C# 15 union: a type with `[System.Runtime.CompilerServices.Union]` that implements `IUnion`. Applications that must still compile on C# 14 (the .NET 10 SDK) declare a portable union (`[Union] partial struct` with one `partial` constructor per case), which LaunchPad's union generator implements in exactly the shape the compiler gives a native `union` declaration. Before .NET 11 the two attribute types come from the Polyfill package, which the LaunchPad SDK adds. When .NET 12 ships and `net10.0` is dropped, a portable union becomes a native one by rewriting its declaration, and nothing that consumes it changes.

## Considered Options

- **Require C# 15 everywhere, polyfill only.** The SDK 11 compiler supports native unions on `net10.0` once the two types exist, so no generator would be needed. Rejected: applications still on the .NET 10 SDK could not declare a union at all.
- **LaunchPad's own union type or API** (`Result<T>`, `OneOf`-style `Match` methods, as in UnionTypes.Toolkit's library). Rejected: every call site would have to be rewritten when moving to native unions, and the specs could not show which errors an operation returns.
- **A non-boxing layout** (UnionTypes.Toolkit's generator). Rejected: a much larger generator whose behaviour differs from the native unions it will be replaced by; such a hand-written custom union is still detected.
- **The generator emits its own polyfill.** Built first, then replaced by Polyfill (2026-11-09): Polyfill already ships the types, embedded and type-forwarded on .NET 11, and is the package the repository uses.

## Consequences

- Generators detect a union by name (`UnionAttribute` or `IUnion`), never by symbol: before .NET 11 every assembly carries its own embedded copy, and Roslyn does not surface `[Union]` through `GetAttributes()` on native or metadata unions.
- Generated code reads a union through `Value` and builds one with its constructor, because a pattern over a union does not compile on C# 14 (CS9327).
- Handlers stop throwing `NotFoundException` and its siblings once every transport answers union results ([#3148](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3148)), which removes the exceptions and their filters and interceptors.
