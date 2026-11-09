# Union types: one standard for .NET 10 and .NET 11+

Status: **designed and built 2026-11-09** under [#3147](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3147). [#3148](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3148) (union results on every API transport) builds on the detection contract in [section 4](#4-the-detection-contract-for-other-generators).

Related: [api-generators-plan.md](api-generators-plan.md) (phase 4, union results), the C# 15 [unions tutorial](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/unions) and [feature specification](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-15.0/unions), and [UnionTypes.Toolkit](https://github.com/mattwar/UnionTypes.Toolkit), whose generator writes custom unions against the same specification.

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

For each `partial struct` with `[Union]` (matched by full name with `ForAttributeWithMetadataName`) that declares at least one `partial` constructor:

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

    public override string ToString() => _value?.ToString() ?? string.Empty;
}
```

- **Storage is one `object?` field**, as in a native `union`: value-type cases box. A non-boxing layout (UnionTypes.Toolkit's overlap/decompose) is out of scope; an application that needs one writes a custom union by hand or uses that generator, and LaunchPad still detects it (section 4).
- **The implicit conversions** give C# 14 code the conversion C# 15 gets from the language. They are emitted on every language version, so a multi-targeted library has the same public API on both target frameworks. They are left out for a case the language doesn't allow a user-defined conversion from (an interface or `object`); C# 15 still converts those.
- Generic unions (`partial struct Result<T>`), nested types and any accessibility are supported. Nullable case types (`partial Maybe(string? value)`) are passed through as written.
- No `Match`/`Switch` methods: native unions have none, and a call to one would have to be rewritten when switching to the native form. C# 14 code reads a union with `Value`:

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

When the compilation has no `System.Runtime.CompilerServices.UnionAttribute` it can use (on `net10.0`), the generator adds `UnionAttribute` and `IUnion` as **internal** types in the project, behind `#if !NET11_0_OR_GREATER`, the way PolySharp does. On `net11.0` it adds nothing. Because the compiler matches them by name (section 1), a project on C# 15 targeting `net10.0` gets full native union support too — LaunchPad's own projects build with `LangVersion` preview and do.

### 3.4 Diagnostics

All are errors on the declaration and stop generation for that type:

| Id       | When                                                                            |
| -------- | ------------------------------------------------------------------------------- |
| LPAD0010 | A `[Union]` type with `partial` case constructors isn't a `struct`.             |
| LPAD0011 | A `partial` constructor of a `[Union]` type doesn't take exactly one parameter. |
| LPAD0012 | Two case constructors take the same type (ignoring nullability).                |

A `[Union]` struct with no `partial` constructors is left alone: it is a native or hand-written custom union.

### 3.5 Standard error cases

`Rocket.Surgery.LaunchPad.Primitives` ships the error cases a handler returns instead of throwing, one `sealed record` each, all implementing `IProblemDetailsData` so every transport can write title, detail and extensions the same way:

| Case           | Replaces                 | HTTP | gRPC                 | GraphQL code   |
| -------------- | ------------------------ | ---- | -------------------- | -------------- |
| `NotFound`     | `NotFoundException`      | 404  | `NotFound`           | `NOT_FOUND`    |
| `Invalid`      | validation exceptions    | 400  | `InvalidArgument`    | `INVALID`      |
| `Unauthorized` | —                        | 401  | `Unauthenticated`    | `UNAUTHORIZED` |
| `Forbidden`    | `NotAuthorizedException` | 403  | `PermissionDenied`   | `FORBIDDEN`    |
| `Conflict`     | `RequestFailedException` | 409  | `FailedPrecondition` | `CONFLICT`     |

The names are the ones phase 4 of the API generators maps by convention (api-generators-plan, section 5). The mapping columns are #3148's to implement; they are listed here so both tickets use the same table.

## 4. The detection contract for other generators

This is what the API generators (#3148) and any other LaunchPad generator rely on. It holds for native unions, unions from LaunchPad's generator, and hand-written custom unions, in source or in a referenced assembly.

1. **A type is a union** when it has an attribute whose class is named `System.Runtime.CompilerServices.UnionAttribute`. Match by full metadata name, never by symbol identity: on `net10.0` every assembly carries its own internal polyfill.
2. **Its cases** are the parameter types of its public instance constructors that take exactly one parameter, in declaration order. For a union from LaunchPad's generator the `partial` constructor declarations are in source, so another generator in the same compilation sees the cases before the implementation exists.
3. **Generated code reads a union through `Value`** with type tests in case order (`value.Value switch { Case1 c => …, Case2 c => …, _ => … }`). That compiles on every language version; a type pattern over the union itself does not compile on C# 14. Generated code builds a union with `new TUnion(caseValue)`, never with a conversion, for the same reason.
4. **A case's meaning comes from its type**: the standard cases of section 3.5 by full name (`Rocket.Surgery.LaunchPad.Primitives.NotFound`, …), other cases by the naming convention and attribute of api-generators-plan phase 4. Any case that is not an error case is the success value.
5. **Unwrapping**: a handler response of `Task<TUnion>`, `ValueTask<TUnion>` or `TUnion` is a union response. A union is never nested in another union's case for this purpose.

`src/Api/Core/UnionShape.cs` implements rules 1, 2 and 4 as `UnionShape.TryCreate(ITypeSymbol, out UnionShape)`, returning equatable case models so it can sit in an incremental pipeline. #3148 uses it; nothing else in `src/Api` does yet.

## 5. Migration

- **Exceptions stay for now.** `NotFoundException`, `NotAuthorizedException`, `RequestFailedException` and their filters and interceptors keep working until #3148 makes every transport answer union results. #3148 then moves the samples' handlers to unions and removes the exceptions, filters and interceptors in the same change, so no build sits between with handlers that can't report errors.
- **No union type in `src/` to migrate**: none existed (section 2).
- **Applications** that hand-wrote OneOf-style or `Result<T>` types can turn them into portable unions: a `partial struct` with `[Union]` and the case constructors. Their consumers move from `IsSuccess`/`Match` to `Value` patterns (C# 14) or plain patterns (C# 15).

## 6. Dropping .NET 10

When .NET 12 ships and LaunchPad drops `net10.0`:

1. Rewrite each portable union `[Union] partial struct X { public partial X(A value); public partial X(B value); }` as `union X(A, B);` — or keep it; the generator still works on C# 15.
2. Delete the polyfill emission (section 3.3).
3. Optionally rewrite `x.Value switch { …, _ => … }` as `x switch { … }` to get exhaustiveness. The analyzer for that is a later nicety, not part of this design.

Nothing in the detection contract changes, so the API generators need no change.

## 7. Decisions

Decided 2026-11-09 (put to the user with AskUserQuestion; recommendation first in each):

- _see the PR for the answers; this list is updated when they are recorded_
