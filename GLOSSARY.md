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
A package that adds LaunchPad support for one third-party technology on one surface, for example NodaTime with Entity Framework, or Newtonsoft.Json with ASP.NET Core. One technology usually has several.
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
The MSBuild SDK an application opts into so that the right LaunchPad packages are chosen from what the project references. It builds on Dovetail's SDK.
_Avoid_: Meta-package, bundle

**Union case**:
One of the types a union response can hold: its value case or one of its error cases.
_Avoid_: Variant, arm, branch

**Portable union**:
A union declared so that it compiles on every supported .NET and C# version, and becomes a native union by changing only its declaration.
_Avoid_: Generated union, polyfilled union

**Native union**:
A union declared with the C# language's own union syntax.
_Avoid_: Built-in union

**Problem**:
One of LaunchPad's standard error cases (not found, invalid, unprocessable, unauthorized, forbidden, conflict), which carries the details every transport writes into its error shape.
_Avoid_: Error result, failure, problem details (that is the HTTP shape of a problem)
