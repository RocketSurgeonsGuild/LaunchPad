# LaunchPad targets .NET 11 only, and its unions are native C# 15 unions

LaunchPad 2.0 targets `net11.0` and later, declared once as `LaunchPadTargetFrameworks` in `Directory.Build.props`, and drops `net10.0`. A union is a native C# 15 `union` declaration, or a hand-written custom union; LaunchPad no longer has a portable union form, a union generator or a polyfill for the union types. The LaunchPad SDK stops a project on an older .NET with `LPADSDK0001`. This supersedes the parts of [ADR-0005](0005-portable-unions-shaped-like-csharp-unions.md) about the portable form, its generator and Polyfill; the rest of ADR-0005 (every LaunchPad union is a C# 15 union, detected by name, and error cases are returned rather than thrown) stands.

ADR-0005 planned to drop `net10.0` when .NET 12 ships. Dropping it in the 2.0 major instead, which is breaking anyway, means LaunchPad uses .NET 11 directly: no generator to keep in step with the compiler (about 860 lines, five diagnostics and two code fixes), no Polyfill added to applications, generated code and tests that don't avoid C# 15, and every test project runs once instead of twice. Dropping a framework after 2.0 would need another major.

## Considered Options

- **Keep `net10.0` until .NET 12** (ADR-0005's plan). Rejected: it keeps the portable form, its generator and Polyfill for a framework the 2.0 release would then have to support for its whole life.
- **Depend on an existing union generator** such as UnionTypes.Toolkit, as #3237 proposed. Rejected: on .NET 11 a native union needs no generator, and ADR-0005's objections (a different shape from the native union it would be replaced by) still apply. A hand-written or generated custom union is still detected.
- **Keep the analyzers that move code to native unions** (LPAD0013, LPAD0014). Rejected: with no portable form left, LaunchPad has nothing for them to migrate; an application that had portable unions rewrites each declaration once (the 2.0 migration notes say how).

## Consequences

- Every `Rocket.Surgery.LaunchPad.*` library package ships `net11.0` assets only, and depends on the 11.0 line of the Microsoft packages. The generators, analyzers, MSBuild task and SDK packages stay on `netstandard2.0`.
- A project on .NET 10 or older that applies the LaunchPad SDK fails with `LPADSDK0001`, which names the minimum. A .NET Standard library is left alone.
- `[Union] partial struct` no longer compiles into a union: an application rewrites it as `union X(A, B);`. LPAD0010 to LPAD0014, `LaunchPadEnablePolyfill` and `LaunchPadPolyfillVersion` are gone.
- The detection contract (docs/design/union-types.md, section 4) is unchanged, except that a native union's implicitly declared case constructors count, which the generators had skipped for a union declared in the same project. Generated code still reads a union through `Value`, which works for every union, native or hand-written.
- Adding `net12.0` is a change to `LaunchPadTargetFrameworks` plus the SDK and the per-framework package versions; `CONTRIBUTING.md` lists every place. The work is tracked by #3239.
