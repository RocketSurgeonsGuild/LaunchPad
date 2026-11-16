---
title: Generating HTTP Clients
description: Generate a typed HTTP client from an OpenAPI document with Kiota or Refitter, registered with IHttpClientFactory by a Joint.
---

The [LaunchPad SDK](../../concepts/launchpad-sdk/) generates HTTP clients from OpenAPI documents when the project builds, with
[Kiota](https://learn.microsoft.com/openapi/kiota/) or [Refitter](https://refitter.github.io/) (which writes
[Refit](https://github.com/reactiveui/refit) interfaces). Each client also gets a generated [Joint](../../concepts/joints/) that
registers it with `IHttpClientFactory`, so the host `ConfigureDovetail()` builds can hand it out.

## Declaring a client

Add an `OpenApiClient` item. Its `Include` is where the document comes from, and `Generator` and `ClientName` are required:

```xml
<ItemGroup>
  <OpenApiClient Include="openapi/rockets.json" Generator="Kiota" ClientName="RocketsClient" Namespace="MyApp.Rockets" />
</ItemGroup>
```

| Metadata              | Meaning                                                                                                                                                                                    |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `Generator`           | `Kiota` or `Refitter`.                                                                                                                                                                     |
| `ClientName`          | A C# identifier. Kiota's client class is called this, Refitter's interface is `I<ClientName>`, the Joint is `<ClientName>Joint`, and the base address is read from `Clients:<ClientName>`. |
| `Namespace`           | The namespace of the generated code. Defaults to the project's `RootNamespace`. Give each client its own when a project has several.                                                       |
| `DocumentName`        | For a project source that writes several documents, the one to use (`v1` is ASP.NET Core's default).                                                                                       |
| `DocumentCache`       | For a URL source, the file the downloaded document is kept in, relative to the project (`obj` when not set). Commit it to build without the URL.                                           |
| `UpdateDocumentCache` | For a URL source, `false` to use the cached document whenever it exists instead of downloading it again. Defaults to `$(LaunchPadUpdateOpenApiDocumentCache)`, which is `true`.            |
| `Options`             | More arguments for the generator's command line: `kiota generate`, or `refitter` after the settings file the SDK writes.                                                                   |

The SDK adds the packages the generated code needs: `Microsoft.Kiota.Bundle` for Kiota, `Refit.HttpClientFactory` for Refitter,
and `Microsoft.Extensions.Http` for both. Like every package it adds, one the project already references is left alone, so
reference it yourself to choose another version.

## Where the document comes from

### A file

A path relative to the project, JSON or YAML (`.json`, `.yaml`, `.yml`):

```xml
<OpenApiClient Include="openapi/rockets.yaml" Generator="Refitter" ClientName="Rockets" />
```

### A server project

A project in the same repository that writes its document when it builds. The server references
`Microsoft.Extensions.ApiDescription.Server`, which runs the application at build time and writes the documents it registers
with `AddOpenApi()` into its `obj` folder:

```xml
<!-- Rockets.Api.csproj -->
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="11.0.0" />
  <PackageReference Include="Microsoft.Extensions.ApiDescription.Server" Version="11.0.0" PrivateAssets="all" />
</ItemGroup>
```

```xml
<!-- the client project -->
<OpenApiClient Include="../Rockets.Api/Rockets.Api.csproj" Generator="Kiota" ClientName="RocketsClient" />
```

The SDK makes the server a project reference that is built first, but not referenced by the client's assembly. A server with
several target frameworks writes its document from its first one. A server that registers several documents needs
`DocumentName`.

### A URL

```xml
<OpenApiClient Include="https://rockets.example.com/openapi/v1.json" Generator="Kiota" ClientName="RocketsClient" />
```

The document is downloaded on every build and kept in a cache, `obj` by default. When the download fails, the build uses the
cached copy, with warning `LPSDK0104`, so a build without the network works once the document has been downloaded. A file or
project source never needs the network once the packages are restored.

To keep the cache with the code, name a file in the project with `DocumentCache` and commit it. A fresh clone, or a CI agent,
then builds even when the URL is down:

```xml
<OpenApiClient
  Include="https://rockets.example.com/openapi/v1.json"
  Generator="Kiota"
  ClientName="RocketsClient"
  DocumentCache="openapi/rockets.json"
/>
```

Every build still downloads the document and updates the cache when the API changed, so the change shows up in your diff. Set
`UpdateDocumentCache="false"` to use the cache whenever it exists and download only when it doesn't: the client is then generated
from the committed document until you update it on purpose (delete the file, or build once with
`-p:LaunchPadUpdateOpenApiDocumentCache=true`), which keeps a breaking change of the API out of your build.
`LaunchPadUpdateOpenApiDocumentCache` sets the default for every client of the project.

## Using the client

The generated Joint registers the client as a typed client, and reads its base address from configuration:

```json
{
    "Clients": {
        "RocketsClient": {
            "BaseAddress": "https://rockets.example.com/"
        }
    }
}
```

Without a base address, a Kiota client uses the first server of its document; a Refit client needs one.

```csharp
using var host = await Host.CreateApplicationBuilder(args).ConfigureDovetail();

// Kiota: the client class
var rockets = host.Services.GetRequiredService<RocketsClient>();
var rocket = await rockets.Rockets[id].GetAsync();

// Refitter: the Refit interface
var rocketsApi = host.Services.GetRequiredService<IRockets>();
var rocket = await rocketsApi.GetRocket(id);
```

The registration is `IHttpClientFactory`'s, so the usual `IHttpClientBuilder` configuration applies on top of it: add handlers or
resilience in a Joint of your own with `services.AddHttpClient("RocketsClient")` for Kiota (the client is named after
`ClientName`), or `services.AddRefitClient<IRockets>()` for Refit. A client generated in a library is registered in every
application that references the library, as any exported Joint is.

To use a client without the host, for example against a test server, create it from an `HttpClient`:

```csharp
var kiota = new RocketsClient(new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient));
var refit = RestService.For<IRockets>(httpClient);
```

## How generation runs

- Both generators are .NET tools. The SDK has NuGet download the tool package with the project's other packages
  (`PackageDownload`) and runs it from the packages folder; nothing is installed globally or in a tool manifest. The versions are
  the ones the SDK was packed with; set `LaunchPadKiotaVersion` or `LaunchPadRefitterVersion` to use another.
- The tools run on .NET 10 or later.
- Generation is incremental. The document is copied into `obj/<configuration>/<framework>/openapi/<ClientName>/` only when its
  content changes, and the client is generated again only when that copy, or the client's settings (generator, its version,
  `ClientName`, `Namespace`, `Options`), change. A server that writes the same document again regenerates nothing.
- The generated code stays in `obj` and is added to the compilation; `dotnet clean` removes it. A design-time build (the IDE)
  only adds what the last build generated.
- Telemetry and update checks of the tools are turned off.

## Errors

| Code        | Meaning                                                                            |
| ----------- | ---------------------------------------------------------------------------------- |
| `LPSDK0100` | `Generator` is missing or is not `Kiota` or `Refitter`.                            |
| `LPSDK0101` | `ClientName` is missing or is not a C# identifier.                                 |
| `LPSDK0102` | The document file does not exist.                                                  |
| `LPSDK0103` | The server project wrote several documents (or none matched `DocumentName`).       |
| `LPSDK0104` | Warning: the URL could not be reached, and the document downloaded before is used. |
| `LPSDK0105` | The URL could not be reached and there is no cached document.                      |
| `LPSDK0106` | The generator tool is not in the packages folder: restore the project.             |
