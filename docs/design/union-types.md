# Union types: native C# 15 unions

Status: **designed and built 2026-11-09** under [#3147](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3147); **portable form, union generator and polyfill removed** by [#3239](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3239), when LaunchPad moved to .NET 11 only ([ADR-0010](../adr/0010-net11-only-and-native-unions.md)). [#3148](https://github.com/RocketSurgeonsGuild/LaunchPad/issues/3148) (union responses on every API transport) builds on the detection contract in [section 4](#4-the-detection-contract-for-other-generators).

Related: [api-generators-plan.md](api-generators-plan.md) (phase 4, union responses), the C# 15 [unions tutorial](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/unions) and [feature specification](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-15.0/unions), and [UnionTypes.Toolkit](https://github.com/mattwar/UnionTypes.Toolkit), whose generator writes custom unions against the same specification.

Contents:

1. [What C# 15 gives us](#1-what-c-15-gives-us)
2. [What LaunchPad has today](#2-what-launchpad-has-today)
3. [Design](#3-design)
4. [The detection contract for other generators](#4-the-detection-contract-for-other-generators)
5. [Migration](#5-migration)
6. [Dropping .NET 10 (done)](#6-dropping-net-10-done)
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
| Does the C# 15 compiler support unions when targeting .NET 10?                                 | Yes, if the compilation can see a `UnionAttribute` and an `IUnion` with those full names (a polyfill). Native `union` declarations and custom unions both work.                                         |
| Does an **internal** polyfill work across assemblies?                                          | Yes. A library and an application that each carry an internal polyfill: the application switches over the library's union with full language support. The compiler matches the types by name.           |
| What does C# 14 (`LangVersion` 14, or the .NET 10 SDK) accept?                                 | A `[Union]` struct, its (partial) constructors, `Value` and user-defined implicit conversions compile. A type pattern over the union itself is CS9327 (_unions not available in C# 14_). `Value` works. |
| Do user-defined implicit conversions from the case types clash with C# 15's union conversions? | No. A custom union that declares them compiles and behaves the same on C# 15.                                                                                                                           |

## 2. What LaunchPad has today

_This section records the state before unions; the exceptions it lists were removed by #3216 (section 5)._

There was no union, result or OneOf-style type in `src/`. Error outcomes were **exceptions** that each transport caught:

| Type                                 | Package    | HTTP (`src/AspNetCore/Filters`) | gRPC (`src/Grpc`)    | GraphQL (`GraphqlErrorFilter`) |
| ------------------------------------ | ---------- | ------------------------------- | -------------------- | ------------------------------ |
| `ProblemDetailsException` (abstract) | Foundation | ProblemDetails                  | ProblemDetails       | error with extensions          |
| `NotFoundException`                  | Foundation | 404                             | `NotFound`           | `NOTFOUND`                     |
| `NotAuthorizedException`             | Foundation | 403                             | `PermissionDenied`   | `NOTAUTHORIZED`                |
| `RequestFailedException`             | Foundation | 400                             | `FailedPrecondition` | `FAILED`                       |

The Sample.Core handlers threw `NotFoundException` for missing entities and `RequestFailedException` for conflicts. The API generators add null → 404 for nullable responses (api-generators-plan, decision 6).

## 3. Design

### 3.1 Declaring a union

A LaunchPad union is a **native C# 15 union**. LaunchPad targets .NET 11 and later, whose BCL has `UnionAttribute` and `IUnion`, so
nothing generates or polyfills it:

```csharp
public union GetRocketResult(RocketModel, NotFound);
```

The compiler writes the struct of section 1: `[Union]`, `IUnion`, one constructor per case, `Value`, the union conversions and the
exhaustive patterns. A **hand-written custom union** (a struct or class with `[Union]`, `IUnion`, a public one-parameter constructor per
case and `Value`) is accepted the same way, for example when an application needs a non-boxing layout; UnionTypes.Toolkit's generator
writes those.

Application code reads a union with C# 15's patterns:

```csharp
return result switch
{
    RocketModel rocket => Ok(rocket),
    NotFound notFound => NotFound(notFound),
};
```

### 3.2 to 3.4 (removed)

Until #3239 LaunchPad also had a **portable form** (`[Union] partial struct` with `partial` case constructors) that compiled on C# 14,
implemented by LaunchPad's union generator, with the union types from the Polyfill package before .NET 11 (added by the LaunchPad SDK), and
diagnostics for it: LPAD0010 to LPAD0014, and a code fix for CS9327. All of it is gone ([ADR-0010](../adr/0010-net11-only-and-native-unions.md));
a portable union becomes `union X(A, B);` by rewriting its declaration, and nothing that consumes it changes.

### 3.5 Standard error cases

`Rocket.Surgery.LaunchPad.Foundation` ships the error cases a handler returns instead of throwing. Each is a `sealed record` deriving from the abstract record `Problem`, which implements `IProblemDetailsData`, so every transport writes detail, title, link, instance and extensions the same way. A union may list the cases it returns, or `Problem` to return any of them.

| Case                  | Replaces                 | HTTP | gRPC                 | GraphQL code      |
| --------------------- | ------------------------ | ---- | -------------------- | ----------------- |
| `NotFound`            | `NotFoundException`      | 404  | `NotFound`           | `NOT_FOUND`       |
| `Invalid`             | `RequestFailedException` | 400  | `InvalidArgument`    | `BAD_USER_INPUT`  |
| `UnprocessableEntity` | validation failures      | 422  | `InvalidArgument`    | `BAD_USER_INPUT`  |
| `Unauthorized`        | —                        | 401  | `Unauthenticated`    | `UNAUTHENTICATED` |
| `Forbidden`           | `NotAuthorizedException` | 403  | `PermissionDenied`   | `FORBIDDEN`       |
| `Conflict`            | `RequestFailedException` | 409  | `FailedPrecondition` | `CONFLICT`        |

- The names are the ones phase 4 of the API generators maps by convention (api-generators-plan, section 5). `NotFound` and `Conflict` share their names with `Microsoft.AspNetCore.Http.HttpResults` types; hand-written code that imports both namespaces uses an alias. Generated code always writes `global::` names.
- The GraphQL codes follow the convention most GraphQL servers and clients use (Apollo's `UNAUTHENTICATED`, `FORBIDDEN`, `BAD_USER_INPUT`, plus `NOT_FOUND` and `CONFLICT`). They replace `GraphqlErrorFilter`'s `NOTFOUND`, `NOTAUTHORIZED` and `FAILED`, a break #3148 lists.
- Validation failures answer **422**, as FluentValidation's do over HTTP already: `UnprocessableEntity` is the validation case, and a case named `Validation…` maps to it. `Invalid` stays 400 for a request that is malformed or can never succeed as sent (decided 2026-11-09, in review).
- `Invalid.Errors` and `UnprocessableEntity.Errors` are `IReadOnlyDictionary<string, string[]>`, the shape of ASP.NET's `ValidationProblemDetails`. FluentValidation failures map into it with a `ToInvalid()` helper (#3148), which puts error codes and attempted values in `Properties`.
- There is no generic `Result<T>`: each operation declares its own union, so its specs show exactly which errors it returns.
- The mapping columns are #3148's to implement; they are listed here so both tickets use the same table.

## 4. The detection contract for other generators

This is what the API generators (#3148) and any other LaunchPad generator rely on. It holds for native unions and hand-written custom unions, in source or in a referenced assembly.

1. **A type is a union** when it has an attribute whose class is named `System.Runtime.CompilerServices.UnionAttribute`, or implements an interface named `System.Runtime.CompilerServices.IUnion`. Match by full name, never by symbol identity: a hand-written union may declare its own copies. Both checks are needed: Roslyn doesn't surface `[Union]` through `GetAttributes()` on a native `union` declaration or on a union read from metadata, and a hand-written union may carry the attribute without implementing `IUnion`.
2. **Its cases** are the parameter types of its public instance constructors that take exactly one parameter, in declaration order. Implicitly declared constructors count: in source, the compiler declares a native union's case constructors implicitly (#3239 fixed the generators, which skipped them).
3. **Generated code reads a union through `Value`** with type tests in case order, through `IUnion` when `Value` isn't public. That works for every union, native or hand-written, and for a nullable `TUnion?`; C# 15's patterns over the union would be no simpler there, so the generators keep it. Generated code builds a union with `new TUnion(caseValue)`.
4. **A case's meaning comes from its type**: an error case by the naming convention or `[ApiError]` of api-generators-plan phase 4, which the standard cases of section 3.5 match by name. The value case is the first case that is not an error case, and a union response has exactly one; another case that is neither is LPAD1028. A `Problem` case stands for each standard case, tested by runtime type, then any other problem (500). A case implementing `IProblemDetailsData` fills the error shape's detail, title, type, instance and extensions from it.
5. **Unwrapping**: a handler response of `Task<TUnion>`, `ValueTask<TUnion>` or `TUnion` is a union response. A union is never nested in another union's case for this purpose.

`src/Analyzers/Api/Core/UnionResults.cs` (`UnionResults.TryRead`) is the one place in the API generators that knows rules 1 and 2; `HostReader` applies rules 4 and 5 through it. The JSON context manifest generator (`src/Analyzers/Json`) has its own copy of rules 1 and 2.

## 5. Migration

- **The samples move to unions** with #3148, in the same pull request. Sample.Core's unary handlers return unions (native since #3239; `GetRocket.Result`: `RocketModel` or `NotFound`; `CreateRocket.Result`: `Response` or `Conflict`; `DeleteRocket.Result`: `Unit` or `NotFound`; …) instead of throwing, and the generated Minimal API, controller and GraphQL hosts answer them. The hand-written callers (the Blazor and Razor Pages UIs, the proto-first gRPC services) split a response with Sample.Core's `TryGetValue<T>()` and answer a problem their own way: a page answers its HTTP status or shows the problem, and a gRPC service ends the call with the case's gRPC status.
- **The rest moved with #3216.** `GetRocketLaunchRecords`, a stream, streams `GetRocketLaunchRecords.Result` (`LaunchRecordModel` or `NotFound`) and answers `NotFound` as its first and only item for a missing rocket, which ends the stream. The patch operations of `EditRocket` and `EditLaunchRecord` implement their handle method themselves, because the generated one (ADR-0004) sends what `GetRequest` loads and can't answer `NotFound`.
- **The exceptions are gone.** #3216 removed `NotFoundException`, `NotAuthorizedException`, `RequestFailedException` and their base `ProblemDetailsException` from Foundation, with the ASP.NET Core filters (and their base `ProblemDetailsExceptionFilter<T>`), the gRPC interceptors (and their base `ProblemDetailsInterceptor<T>`), their cases in HotChocolate's `GraphqlErrorFilter` and their status codes in the `ProblemDetailsJoint`. Throwing is not a supported way to report a failure: an exception answers as an unexpected error.
- **No union type in `src/` to migrate**: none existed (section 2).
- **Applications** that hand-wrote OneOf-style or `Result<T>` types can turn them into native unions, `union X(A, B);`. Their consumers move from `IsSuccess`/`Match` to C# 15's patterns.
- **Applications with portable unions** (LaunchPad 2.0 previews before #3239) rewrite each `[Union] partial struct X { public partial X(A value); public partial X(B value); }` as `union X(A, B);`, and may turn `x.Value switch { …, _ => … }` into an exhaustive `x switch { … }`.

## 6. Dropping .NET 10 (done)

Planned for .NET 12, done for LaunchPad 2.0 by #3239 ([ADR-0010](../adr/0010-net11-only-and-native-unions.md)), which made LaunchPad target
.NET 11 only:

1. LaunchPad's own unions (the samples and the test fixtures) became native `union` declarations.
2. The union generator, its analyzer, code fixes and diagnostics (LPAD0010 to LPAD0014) were deleted, with no replacement generator.
3. The LaunchPad SDK stopped adding Polyfill (section 3.3 before #3239), and `LaunchPadEnablePolyfill` and `LaunchPadPolyfillVersion` went with it.

The detection contract didn't change, except that implicitly declared case constructors now count (rule 2).

## 7. Decisions

Decided 2026-11-09 with the user (AskUserQuestion, then a grilling session); the lasting choice is recorded in [ADR-0005](../adr/0005-portable-unions-shaped-like-csharp-unions.md); the transport side in [ADR-0006](../adr/0006-union-responses-over-every-transport.md).

1. _(Superseded by ADR-0010, #3239.)_ **Declaration on .NET 10**: the portable form, `[Union] partial struct` with `partial` case constructors, implemented by a generator. Native `union` is accepted wherever C# 15 is.
2. **Storage**: one `object?` field, as in a native union.
3. _(Superseded by ADR-0010, #3239.)_ **Polyfill**: from the Polyfill package with `PolyUseEmbeddedAttribute`, added by the LaunchPad SDK to .NET 10 projects that get Foundation (opt out with `LaunchPadEnablePolyfill=false`). Replaced the generator's own embedded copy, which was built first.
4. _(Superseded by ADR-0010, #3239: the generator is gone.)_ **Packaging**: the union generator and analyzers are in LaunchPad's one analyzer assembly (`src/Analyzers`), carried by Foundation. (First their own assembly packed in Primitives; moved when Primitives became the optional strong types package, #3166.)
5. **Standard cases**: `Problem` plus `NotFound`, `Invalid`, `UnprocessableEntity`, `Unauthorized`, `Forbidden`, `Conflict`, short names kept despite the `HttpResults` clash. `Invalid.Errors` is a `string[]` dictionary. No generic `Result<T>`.
6. **Value cases**: a union response has exactly one value case; any other non-error case is LPAD1028. (First decided as "several", revised once #3148 showed each transport has one response type: gRPC would need a generated `oneof` message and GraphQL a union output type.)
7. **GraphQL codes**: the common convention (`NOT_FOUND`, `BAD_USER_INPUT`, `UNAUTHENTICATED`, `FORBIDDEN`, `CONFLICT`).
8. _(Superseded by ADR-0010, #3239.)_ **Analyzers in this change**: the CS9327 code fix (C# 14), LPAD0013 as a warning and LPAD0014 as info (C# 15).
9. _(Superseded by ADR-0010, #3239.)_ **Missing polyfill**: LPAD0012, an error.
10. **Migration**: the samples' handlers move to unions in the combined #3147/#3148 change; the exceptions, filters and interceptors stayed until a follow-up, #3216, which moved the last throwing handlers to unions and removed them.
11. **Problem data**: the transports use `IProblemDetailsData` when an error case implements it (HTTP ProblemDetails fields and extensions; the gRPC status message; the GraphQL message and extensions).
12. **Validation is 422**: a new `UnprocessableEntity` problem and the `Validation` name rule answer 422; `Invalid` stays 400.
13. **Terms**: union response, union case, value case, error case, native union, problem (GLOSSARY.md); _portable union_ retired with #3239.
14. **.NET 11 only, native unions** (#3239, ADR-0010): no portable form, no union generator, no polyfill.
