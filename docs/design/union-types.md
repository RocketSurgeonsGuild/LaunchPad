# Union types: one standard for .NET 10 and .NET 11+

Status: **designed and built 2026-11-09** under [#3147](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3147). [#3148](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3148) (union responses on every API transport) builds on the detection contract in [section 4](#4-the-detection-contract-for-other-generators).

Related: [api-generators-plan.md](api-generators-plan.md) (phase 4, union responses), the C# 15 [unions tutorial](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/unions) and [feature specification](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-15.0/unions), and [UnionTypes.Toolkit](https://github.com/mattwar/UnionTypes.Toolkit), whose generator writes custom unions against the same specification.

Contents:

1. [What C# 15 gives us](#1-what-c-15-gives-us)
2. [What LaunchPad has today](#2-what-launchpad-has-today)
3. [Design](#3-design)
4. [The detection contract for other generators](#4-the-detection-contract-for-other-generators)
5. [Migration](#5-migration)
6. [Dropping .NET 10](#6-dropping-net-10)
7. [Decisions](#7-decisions)

---

## 1. What C# 15 gives us

C# 15 (the .NET 11 SDK) adds unions in two forms. Both were checked on SDK 11.0.100-rc.3 (spikes, 2026-11-09).

**The declaration.** `public union GetRocketResult(RocketModel, NotFound);` lowers to a struct that:

- carries `[System.Runtime.CompilerServices.Union]` and implements `System.Runtime.CompilerServices.IUnion` (`object? Value { get; }`);
- has one public constructor per case, taking the case value;
- stores the value in one `object?` field, so value-type cases box.

**A custom union.** Any struct or class with `[Union]`, `IUnion`, a public one-parameter constructor per case and `Value` gets the same language support. Optional `HasValue` and `TryGetValue(out TCase)` members let the compiler match a case without going through `Value` (so a non-boxing layout stays non-boxing).

The language support is the same for both:

- an implicit _union conversion_ from each case type (`GetRocketResult r = rocket;`, `return new NotFound(id);` in a method returning the union);
- type patterns apply to the contents: `r switch { RocketModel m => …, NotFound n => … }`;
- the switch is exhaustive over the cases, with a `null` arm only when a case type is nullable. `default(TUnion)` holds `null` and falls through such a switch at runtime, as it does for a native union.

What the spikes established:

| Question                                                                                       | Answer                                                                                                                                                                                                  |
| ---------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Are `UnionAttribute` and `IUnion` in the .NET 11 BCL?                                          | Yes, `System.Runtime.dll`, namespace `System.Runtime.CompilerServices`.                                                                                                                                 |
| Does the C# 15 compiler support unions when targeting `net10.0`?                               | Yes, if the compilation can see a `UnionAttribute` and an `IUnion` with those full names (a polyfill). Native `union` declarations and custom unions both work.                                         |
| Does an **internal** polyfill work across assemblies?                                          | Yes. A library and an application that each carry an internal polyfill: the application switches over the library's union with full language support. The compiler matches the types by name.           |
| What does C# 14 (`LangVersion` 14, or the .NET 10 SDK) accept?                                 | A `[Union]` struct, its (partial) constructors, `Value` and user-defined implicit conversions compile. A type pattern over the union itself is CS9327 (_unions not available in C# 14_). `Value` works. |
| Do user-defined implicit conversions from the case types clash with C# 15's union conversions? | No. A custom union that declares them compiles and behaves the same on C# 15.                                                                                                                           |

## 2. What LaunchPad has today

There is no union, result or OneOf-style type in `src/`. Error outcomes are **exceptions** that each transport catches:

| Type                                 | Package    | HTTP (`src/AspNetCore/Filters`) | gRPC (`src/Grpc`)    | GraphQL (`GraphqlErrorFilter`) |
| ------------------------------------ | ---------- | ------------------------------- | -------------------- | ------------------------------ |
| `ProblemDetailsException` (abstract) | Primitives | ProblemDetails                  | ProblemDetails       | error with extensions          |
| `NotFoundException`                  | Primitives | 404                             | `NotFound`           | `NOTFOUND`                     |
| `NotAuthorizedException`             | Primitives | 403                             | `PermissionDenied`   | `NOTAUTHORIZED`                |
| `RequestFailedException`             | Primitives | 400                             | `FailedPrecondition` | `FAILED`                       |

The Sample.Core handlers throw `NotFoundException` for missing entities and `RequestFailedException` for conflicts. The API generators add null → 404 for nullable responses (api-generators-plan, decision 6).

## 3. Design

### 3.1 One declaration form for every target

A LaunchPad union is a **C# 15 union**: a type with `[Union]`. It is declared one of two ways.

**The portable form** compiles on C# 14 and C# 15, on `net10.0` and `net11.0`: a `partial struct` with `[Union]` and one `partial` constructor per case. LaunchPad's union generator implements it.

```csharp
using System.Runtime.CompilerServices;

[Union]
public readonly partial struct GetRocketResult
{
    public partial GetRocketResult(RocketModel value);
    public partial GetRocketResult(NotFound value);
}
```

**The native form** needs C# 15: `public union GetRocketResult(RocketModel, NotFound);`. The generator does nothing for it; the compiler writes it.

Both produce the same shape: `[Union]`, `IUnion`, one constructor per case, `Value`. So every consumer — application code and other generators — treats them the same way, and switching from the portable form to the native one later changes the declaration only.

### 3.2 What the generator writes

The union generator is its own analyzer assembly, `Rocket.Surgery.LaunchPad.Primitives.Analyzers` (`src/Primitives/Analyzers`), packed into the Primitives package beside the standard cases it pairs with. For each `partial struct` with `[Union]` (matched by full name with `ForAttributeWithMetadataName`) that declares at least one `partial` constructor:

```csharp
partial struct GetRocketResult : global::System.Runtime.CompilerServices.IUnion
{
    private readonly object? _value;

    public partial GetRocketResult(RocketModel value) => _value = value;
    public partial GetRocketResult(NotFound value) => _value = value;

    /// <summary>The case value, or <see langword="null" /> for <c>default</c>.</summary>
    public object? Value => _value;

    public bool HasValue => _value is not null;

    public bool TryGetValue([MaybeNullWhen(false)] out RocketModel value) { … }
    public bool TryGetValue([MaybeNullWhen(false)] out NotFound value) { … }

    public static implicit operator GetRocketResult(RocketModel value) => new(value);
    public static implicit operator GetRocketResult(NotFound value) => new(value);
}
```

- **Storage is one `object?` field**, as in a native `union`: value-type cases box. A non-boxing layout (UnionTypes.Toolkit's overlap/decompose) is out of scope; an application that needs one writes a custom union by hand or uses that generator, and LaunchPad still detects it (section 4).
- **The implicit conversions** give C# 14 code the conversion C# 15 gets from the language. They are emitted on every language version, so a multi-targeted library has the same public API on both target frameworks. They are left out for a case the language doesn't allow a user-defined conversion from (an interface or `object`), and for a non-public case constructor; C# 15 still converts those.
- Generic unions (`partial struct Result<T>`), record structs, nested types and any accessibility are supported. Nullable case types (`partial Reading(string? text)`) are passed through as written.
- No `Match`/`Switch` methods and no `ToString` override: native unions have none, and a call to one would have to be rewritten when switching to the native form. C# 14 code reads a union with `Value`:

```csharp
// C# 14 and C# 15
return result.Value switch
{
    RocketModel rocket => Ok(rocket),
    NotFound notFound => NotFound(notFound),
    _ => throw new UnreachableException(),
};

// C# 15 only: exhaustive, no discard arm
return result switch
{
    RocketModel rocket => Ok(rocket),
    NotFound notFound => NotFound(notFound),
};
```

### 3.3 The polyfill

Before .NET 11, `UnionAttribute` and `IUnion` come from the [Polyfill](https://github.com/SimonCropp/Polyfill) package (11.4.5 and later), with `PolyUseEmbeddedAttribute` so its copies are embedded and no other assembly sees them, even through `InternalsVisibleTo`. On .NET 11 Polyfill type-forwards them to the BCL. Because the compiler matches the types by name (section 1), a project on C# 15 targeting `net10.0` gets full native union support too.

- **The LaunchPad SDK adds it**: a `net10.0` project the SDK gives `Rocket.Surgery.LaunchPad.Primitives` also gets Polyfill, unless it already references Polyfill or PolySharp. Before .NET 11 the SDK also turns on `PolyUseEmbeddedAttribute`, unless the project asks for `PolyPublic`. `LaunchPadEnablePolyfill=false` opts out. Polyfill has no per-feature include, so the project also gets Polyfill's other polyfills for its target framework, all internal.
- **Without the SDK** an application references Polyfill (or PolySharp) itself.
- The union generator doesn't emit the types. A first version did; it was replaced by Polyfill (decision 3).

### 3.4 Diagnostics and code fixes

The union generator reports LPAD0010 and LPAD0011 on the declaration, and each stops generation for that type; the union analyzer reports LPAD0012 on the attribute:

| Id       | Severity | When                                                                                                                                                    |
| -------- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| LPAD0010 | Error    | A `[Union]` type with `partial` case constructors isn't a `struct`.                                                                                     |
| LPAD0011 | Error    | A `partial` constructor of a `[Union]` type doesn't take exactly one parameter.                                                                         |
| LPAD0012 | Error    | `[Union]` doesn't bind: the project targets a framework before .NET 11 and has no polyfill (reference Polyfill or PolySharp, or use the LaunchPad SDK). |

A containing type that isn't `partial` is the existing LPAD0001. A `[Union]` struct with no `partial` constructors is left alone: it is a native or hand-written custom union.

The analyzer in the same assembly also covers reading unions and the move to native unions. LPAD0013 and LPAD0014 apply only on C# 15; LPAD0013 is reported only when its fix is safe: every arm tests a type a case converts to or from, and the only discard arm is a last one that throws.

| Id                | Severity | When                                                                                             | Code fix                                           |
| ----------------- | -------- | ------------------------------------------------------------------------------------------------ | -------------------------------------------------- |
| CS9327 (compiler) | Error    | C# 14: a pattern or `switch` over a union.                                                       | Switch over `Value` instead, adding a discard arm. |
| LPAD0013          | Warning  | C# 15: a `switch` or `is` over a union's `Value`, which the union's own patterns now cover.      | Switch over the union, dropping the discard arm.   |
| LPAD0014          | Info     | C# 15: a portable union whose declaration is only `[Union]` and its `partial` case constructors. | Rewrite it as `union X(A, B);`.                    |

### 3.5 Standard error cases

`Rocket.Surgery.LaunchPad.Primitives` ships the error cases a handler returns instead of throwing. Each is a `sealed record` deriving from the abstract record `Problem`, which implements `IProblemDetailsData`, so every transport writes detail, title, link, instance and extensions the same way. A union may list the cases it returns, or `Problem` to return any of them.

| Case           | Replaces                 | HTTP | gRPC                 | GraphQL code      |
| -------------- | ------------------------ | ---- | -------------------- | ----------------- |
| `NotFound`     | `NotFoundException`      | 404  | `NotFound`           | `NOT_FOUND`       |
| `Invalid`      | validation exceptions    | 400  | `InvalidArgument`    | `BAD_USER_INPUT`  |
| `Unauthorized` | —                        | 401  | `Unauthenticated`    | `UNAUTHENTICATED` |
| `Forbidden`    | `NotAuthorizedException` | 403  | `PermissionDenied`   | `FORBIDDEN`       |
| `Conflict`     | `RequestFailedException` | 409  | `FailedPrecondition` | `CONFLICT`        |

- The names are the ones phase 4 of the API generators maps by convention (api-generators-plan, section 5). `NotFound` and `Conflict` share their names with `Microsoft.AspNetCore.Http.HttpResults` types; hand-written code that imports both namespaces uses an alias. Generated code always writes `global::` names.
- The GraphQL codes follow the convention most GraphQL servers and clients use (Apollo's `UNAUTHENTICATED`, `FORBIDDEN`, `BAD_USER_INPUT`, plus `NOT_FOUND` and `CONFLICT`). They replace `GraphqlErrorFilter`'s `NOTFOUND`, `NOTAUTHORIZED` and `FAILED`, a break #3148 lists.
- `Invalid.Errors` is `IReadOnlyDictionary<string, string[]>`, the shape of ASP.NET's `ValidationProblemDetails`. FluentValidation failures map into it with a `ToInvalid()` helper (#3148), which puts error codes and attempted values in `Properties`.
- There is no generic `Result<T>`: each operation declares its own union, so its specs show exactly which errors it returns.
- The mapping columns are #3148's to implement; they are listed here so both tickets use the same table.

## 4. The detection contract for other generators

This is what the API generators (#3148) and any other LaunchPad generator rely on. It holds for native unions, unions from LaunchPad's generator, and hand-written custom unions, in source or in a referenced assembly.

1. **A type is a union** when it has an attribute whose class is named `System.Runtime.CompilerServices.UnionAttribute`, or implements an interface named `System.Runtime.CompilerServices.IUnion`. Match by full name, never by symbol identity: before .NET 11 every assembly carries its own embedded copy. Both checks are needed: Roslyn doesn't surface `[Union]` through `GetAttributes()` on a native `union` declaration or on a union read from metadata, and a portable union has no `IUnion` until the union generator runs.
2. **Its cases** are the parameter types of its public instance constructors that take exactly one parameter, in declaration order. For a portable union the `partial` constructor declarations are in source, so another generator in the same compilation sees the cases before the implementation exists.
3. **Generated code reads a union through `Value`** with type tests in case order (`value.Value switch { Case1 c => …, Case2 c => …, _ => … }`). That compiles on every language version; a type pattern over the union itself does not compile on C# 14 (CS9327). Generated code builds a union with `new TUnion(caseValue)`, never with a conversion, for the same reason.
4. **A case's meaning comes from its type**: an error case by the naming convention or `[ApiError]` of api-generators-plan phase 4, which the standard cases of section 3.5 match by name. The value case is the first case that is not an error case, and a union response has exactly one; another case that is neither is LPAD1025. A `Problem` case stands for each standard case, tested by runtime type, then any other problem (500). A case implementing `IProblemDetailsData` fills the error shape's detail, title, type, instance and extensions from it.
5. **Unwrapping**: a handler response of `Task<TUnion>`, `ValueTask<TUnion>` or `TUnion` is a union response. A union is never nested in another union's case for this purpose.

`src/Api/Core/UnionResults.cs` (`UnionResults.TryRead`) is the one place in the API generators that knows rules 1 and 2; `HostReader` applies rules 4 and 5 through it. The union generator's analyzer has its own copy of rule 1 (`UnionNames.IsUnion`), since it is a different assembly.

## 5. Migration

- **The samples move to unions** with #3148, in the same pull request. Sample.Core's unary handlers return portable unions (`GetRocket.Result`: `RocketModel` or `NotFound`; `CreateRocket.Result`: `Response` or `Conflict`; `DeleteRocket.Result`: `Unit` or `NotFound`; …) instead of throwing, and the generated Minimal API, controller and GraphQL hosts answer them. The hand-written callers (the Blazor and Razor Pages UIs, the proto-first gRPC services) read the value case with Sample.Core's `Expect<T>()`, which throws the exception LaunchPad's filters and interceptors already answer for a problem. `GetRocketLaunchRecords`, a stream, still throws `NotFoundException` before its first item: a stream of unions answers an error case per item, not for the stream as a whole.
- **The exceptions stay.** `NotFoundException`, `NotAuthorizedException`, `RequestFailedException` and their filters and interceptors keep working for hand-written code and handlers that don't return unions; removing them is a follow-up (decision 10).
- **No union type in `src/` to migrate**: none existed (section 2).
- **Applications** that hand-wrote OneOf-style or `Result<T>` types can turn them into portable unions: a `partial struct` with `[Union]` and the case constructors. Their consumers move from `IsSuccess`/`Match` to `Value` patterns (C# 14) or plain patterns (C# 15).

## 6. Dropping .NET 10

When .NET 12 ships and LaunchPad drops `net10.0`:

1. Apply LPAD0014's fix to each portable union (`[Union] partial struct X { public partial X(A value); public partial X(B value); }` becomes `union X(A, B);`), or keep it; the generator still works on C# 15.
2. Apply LPAD0013's fix to each `x.Value switch { …, _ => … }`, which becomes an exhaustive `x switch { … }`.
3. Stop adding Polyfill in the LaunchPad SDK for unions (section 3.3).

Nothing in the detection contract changes, so the API generators need no change.

## 7. Decisions

Decided 2026-11-09 with the user (AskUserQuestion, then a grilling session); the lasting choice is recorded in [ADR-0005](../adr/0005-portable-unions-shaped-like-csharp-unions.md); the transport side in [ADR-0006](../adr/0006-union-responses-over-every-transport.md).

1. **Declaration on .NET 10**: the portable form, `[Union] partial struct` with `partial` case constructors, implemented by a generator. Native `union` is accepted wherever C# 15 is.
2. **Storage**: one `object?` field, as in a native union.
3. **Polyfill**: from the Polyfill package with `PolyUseEmbeddedAttribute`, added by the LaunchPad SDK to `net10.0` projects that get Primitives (opt out with `LaunchPadEnablePolyfill=false`). Replaced the generator's own embedded copy, which was built first.
4. **Packaging**: the union generator and analyzers are their own analyzer assembly (`src/Primitives/Analyzers`), packed in Primitives.
5. **Standard cases**: `Problem` plus `NotFound`, `Invalid`, `Unauthorized`, `Forbidden`, `Conflict`, short names kept despite the `HttpResults` clash. `Invalid.Errors` is a `string[]` dictionary. No generic `Result<T>`.
6. **Value cases**: a union response has exactly one value case; any other non-error case is LPAD1025. (First decided as "several", revised once #3148 showed each transport has one response type: gRPC would need a generated `oneof` message and GraphQL a union output type.)
7. **GraphQL codes**: the common convention (`NOT_FOUND`, `BAD_USER_INPUT`, `UNAUTHENTICATED`, `FORBIDDEN`, `CONFLICT`).
8. **Analyzers in this change**: the CS9327 code fix (C# 14), LPAD0013 as a warning and LPAD0014 as info (C# 15).
9. **Missing polyfill**: LPAD0012, an error.
10. **Migration**: the samples' handlers move to unions in the combined #3147/#3148 change; the exceptions, filters and interceptors stay until a follow-up.
11. **Problem data**: the transports use `IProblemDetailsData` when an error case implements it (HTTP ProblemDetails fields and extensions; the gRPC status message; the GraphQL message and extensions).
12. **Terms**: union response, union case, value case, error case, portable union, native union, problem (GLOSSARY.md).
