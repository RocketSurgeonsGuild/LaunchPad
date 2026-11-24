# LaunchPadHealthCheckOptions Properties

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [LaunchPadHealthCheckOptions](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions.md)
- Kind: `Properties`
- Members: `5`

Where LaunchPad maps the health check endpoints ( `MapLaunchPadHealthChecks`), and on which port.

<a id="livepath"></a>

## LivePath

- Name: `LivePath`
- Modifiers: `get; set`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3341-merge.Sha.69b2d11eb32071e6ab9f05439eb8c02f3ae46ce6/src/AspNetCore/HealthChecks/LaunchPadHealthCheckOptions.cs)

The liveness path, under [LaunchPadHealthCheckOptions.Path](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#path). The default is `live`.

```csharp
public string LivePath { get; set; }
```

<a id="path"></a>

## Path

- Name: `Path`
- Modifiers: `get; set`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3341-merge.Sha.69b2d11eb32071e6ab9f05439eb8c02f3ae46ce6/src/AspNetCore/HealthChecks/LaunchPadHealthCheckOptions.cs)

The path of the health report, which the liveness and readiness paths sit under. The default is `/health`.

```csharp
public string Path { get; set; }
```

<a id="port"></a>

## Port

- Name: `Port`
- Modifiers: `nullable` `get; set`
- Returns: `int?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3341-merge.Sha.69b2d11eb32071e6ab9f05439eb8c02f3ae46ce6/src/AspNetCore/HealthChecks/LaunchPadHealthCheckOptions.cs)

The port the health check endpoints are served on, alone. When it is `null` (the default) they are served on the application's own addresses.

```csharp
public int? Port { get; set; }
```

## Remarks

When a port is set, Kestrel also listens on `http://*:{Port}`, next to the application's own addresses. A request on that port for anything but the health check endpoints gets a 404, and so does a request for the health check endpoints on any other port. When the server doesn't listen on the port after all (a `Listen` in code, IIS, HTTP.sys or the in-memory test server), the health check endpoints answer on the application's own addresses, as they do with no port.

<a id="readypath"></a>

## ReadyPath

- Name: `ReadyPath`
- Modifiers: `get; set`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3341-merge.Sha.69b2d11eb32071e6ab9f05439eb8c02f3ae46ce6/src/AspNetCore/HealthChecks/LaunchPadHealthCheckOptions.cs)

The readiness path, under [LaunchPadHealthCheckOptions.Path](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#path). The default is `ready`.

```csharp
public string ReadyPath { get; set; }
```

<a id="resultstatuscodes"></a>

## ResultStatusCodes

- Name: `ResultStatusCodes`
- Modifiers: `get`
- Returns: `IDictionary<HealthStatus, int>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3341-merge.Sha.69b2d11eb32071e6ab9f05439eb8c02f3ae46ce6/src/AspNetCore/HealthChecks/LaunchPadHealthCheckOptions.cs#L58)

The status code each endpoint answers for each `HealthChecks.HealthStatus`. The defaults are 200 for `HealthStatus.Healthy` and `HealthStatus.Degraded`, and 503 for `HealthStatus.Unhealthy`.

```csharp
public IDictionary<HealthStatus, int> ResultStatusCodes { get; }
```
