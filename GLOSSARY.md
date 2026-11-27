# LaunchPad

A simple starting point for building .NET applications. It assembles Specular (assembly scanning) and Dovetail (build-time composition) into ready-to-use wiring for common application types and libraries.

## Language

**Joint**:
A unit of application wiring that Dovetail resolves and applies at build time. LaunchPad's replacement for what it previously called a convention.
_Avoid_: Convention, module

**Host model**:
The kind of application a project is (generic host, ASP.NET Core, Blazor WebAssembly, MAUI or Uno), decided at build time from its SDK and referenced assemblies. It determines which joints apply.
_Avoid_: Environment, host type

**Integration package**:
A package that adds LaunchPad support for one third-party technology on one surface, for example NodaTime with HotChocolate, or ZeroAlloc.Validation with gRPC. One technology usually has several.
_Avoid_: Plugin, add-on, extension

**Direct reference**:
A package or project reference a project declares itself, as opposed to one it only receives transitively. LaunchPad's templates and SDK detection act only on direct references.
_Avoid_: Direct dependency

**Template**:
Source shipped inside a package and written into a consuming project only when its host model and referenced libraries match, so unused wiring never compiles in.
_Avoid_: Snippet, scaffold

**Operation**:
A mediator request, with its response, that is exposed over one or more transports. It is the same thing whether it is reached as an HTTP endpoint, a gRPC method or a GraphQL field.
_Avoid_: Endpoint, contract, action

**Patch operation**:
An operation whose request carries only the changes to another operation's request. It completes by applying those changes to the current request and sending that operation, which validates the result.
_Avoid_: Partial update, patch request

**Contracts project**:
The project that holds operations and their response types, shared by the servers that expose them.
_Avoid_: Shared project, API project

**Transport**:
A way of carrying an operation between a client and a server: Minimal API, MVC controllers, gRPC or GraphQL.
_Avoid_: Protocol, channel

**Operation host**:
A server type that declares, one by one, the operations it exposes over one transport. An operation is only reachable where a host declares it.
_Avoid_: Marker, mapper, endpoint group, API surface

**Group**:
The name an operation host gives its operations: the middle of their routes, the gRPC package and service.
_Avoid_: Area, module, route prefix

**Union response**:
An operation's response that is one of a fixed set of cases, one value case and any number of error cases, so the handler reports an error by returning it instead of throwing.
_Avoid_: Union result, result type, OneOf, discriminated result

**Value case**:
The case of a union response that answers as the operation's successful response.
_Avoid_: Success case, ok case

**Error case**:
A case of a union response that answers as an error on every transport, carrying its case name and its own status.
_Avoid_: Failure, error result, fault

**Mediator**:
The library an application chooses to dispatch its operations. LaunchPad depends on none.
_Avoid_: MediatR (as a generic term), bus, dispatcher

**Mediator profile**:
The description of how one mediator library shapes its requests, dispatches them and declares handlers, which lets operations work with any mediator.
_Avoid_: Mediator adapter, mediator configuration

**LaunchPad SDK**:
The MSBuild SDK an application opts into so that the right LaunchPad packages are chosen from what the project references. It builds on Dovetail's SDK. `Rocket.Surgery.LaunchPad.Sdk` chains `Joinery.Dovetail.Sdk`; `Rocket.Surgery.LaunchPad.Sdk.Console`, for command line applications on ConsoleAppFramework, chains `Joinery.Dovetail.Sdk.Console`; `Rocket.Surgery.LaunchPad.Sdk.Web` and the other mirrors of the Microsoft SDKs (Worker, Razor, BlazorWebAssembly, WebAssembly, WindowsDesktop) each take the place of the Microsoft SDK of the same name and chain Dovetail's mirror of it.
_Avoid_: Meta-package, bundle

**Union case**:
One of the types a union response can hold: its value case or one of its error cases.
_Avoid_: Variant, arm, branch

**Native union**:
A union declared with the C# language's own union syntax, `union X(A, B);`. Every LaunchPad union is one, or a hand-written custom union of the same shape.
_Avoid_: Built-in union, portable union (the `[Union] partial struct` form LaunchPad had before it targeted .NET 11 only), generated union, polyfilled union

**Problem**:
One of LaunchPad's standard error cases (not found, invalid, unprocessable, unauthorized, forbidden, conflict), which carries the details every transport writes into its error shape.
_Avoid_: Error result, failure, problem details (that is the HTTP shape of a problem)

**Policy**:
A named rule that decides whether a caller may run an operation, declared on the operation's request. Most are the scopes they require and nothing else; one with a rule of its own is written by hand.
_Avoid_: Permission, requirement, role (one thing a policy may check), authorization rule

**Scope**:
A named grant in the caller's token that a policy requires. An operation's scopes are published in each transport's contract.
_Avoid_: Permission, claim (a scope is one kind), role

**Security context**:
Who is calling, as policies see them: an identity with its roles and claims, the same on every transport.
_Avoid_: User, principal (that is ASP.NET Core's shape of it), identity

**Edge**:
Where a transport receives a call, before the request is bound and dispatched. Policies are checked there and in the mediator pipeline, and run once.
_Avoid_: Middleware, gateway, front door

**JSON context**:
The JSON metadata an application or package hands to LaunchPad, prepared at build time, so its types are read and written without reflection. LaunchPad uses only the JSON contexts it is given.
_Avoid_: Serializer context, type resolver, JSON options

**JSON context manifest**:
The list of its types an assembly declares as crossing the wire as JSON, from which the applications that reference it get their JSON context.
_Avoid_: JSON registry, type list

**Domain primitive**:
A type that holds a value of a domain concept (an email address, an amount, an order's ID) and only accepts values its rules allow.
_Avoid_: Strong type (LaunchPad's earlier name for it; say domain primitive), wrapper, primitive on its own (that is a built-in type such as `int` or `string`, and Primitives is LaunchPad's package)

**Value object**:
A domain primitive that holds several values, validated together, such as an amount of money and its currency.
_Avoid_: Composite primitive, compound value, record (that is the C# declaration)

**Smart enum**:
A domain primitive whose values are a fixed set of named members, each with a value of its own and, optionally, data of its own (a label, a fee). Depending on how the smart enum is declared, a member can also behave differently from the other members.
_Avoid_: Enumeration class, enum (that is the C# `enum`), case (that is a union's)

**Strong ID**:
A domain primitive that identifies an entity, so the IDs of different entities can't be mixed up.
_Avoid_: Strongly typed ID, typed ID, entity key

**Primitive kind**:
A ready-made set of rules, such as email address, currency code or latitude, applied to a domain primitive the application declares.
_Avoid_: Shortcut (the name in the library Primitives was ported from; say primitive kind), ready-made type, preset

**Rules**:
The checks and normalizations that decide which values a domain primitive accepts, declared on it directly or brought by its primitive kind.
_Avoid_: Template (that is source written into a project), validator (that is one rule written as code), profile, constraints
