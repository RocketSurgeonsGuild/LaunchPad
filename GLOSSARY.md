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

**LaunchPad SDK**:
The MSBuild SDK an application opts into so that the right LaunchPad packages are chosen from what the project references. It builds on Dovetail's SDK.
_Avoid_: Meta-package, bundle
