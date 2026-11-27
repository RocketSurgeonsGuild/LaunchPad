# Health checks

`Rocket.Surgery.LaunchPad.AspNetCore` maps the health check endpoints an ASP.NET Core application needs, and can serve them on a port
the application's own endpoints are not on. Its `HealthChecksJoint` registers the health check services (`AddHealthChecks`) and the
endpoints' options, so an application only maps them:

```csharp
using Rocket.Surgery.LaunchPad.AspNetCore;

var app = await WebApplication.CreateBuilder(args).ConfigureDovetail();
app.MapLaunchPadHealthChecks();
await app.RunAsync();
```

## The endpoints

| Path            | Runs               | Answers                                                              |
| --------------- | ------------------ | -------------------------------------------------------------------- |
| `/health`       | every health check | the report as JSON, in the shape the HealthChecks.UI dashboard reads |
| `/health/live`  | no health check    | `Healthy` as long as the application answers                         |
| `/health/ready` | every health check | their overall status as plain text                                   |

Each answers 200 when the status is `Healthy` or `Degraded`, and 503 when it is `Unhealthy`. Every endpoint allows anonymous requests,
even under a fallback authorization policy. `MapLaunchPadHealthChecks` returns a builder for all three, so
`app.MapLaunchPadHealthChecks().RequireAuthorization("ops")` secures them all.

Add the checks themselves with `AddHealthChecks()`, in the host's services or in a [Joint](../writing-a-joint/):

```csharp
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");
```

Readiness runs every registered check, so a check needs no tag to keep a pod out of rotation while a dependency is down. Liveness runs
none, so a failing dependency never gets a pod restarted.

### The health report

`/health` is written by `LaunchPadHealthCheckResponseWriter`, which writes the report with a `Utf8JsonWriter`, never through reflection
([JSON and native AOT](../json-and-native-aot/)):

```json
{
    "status": "Unhealthy",
    "totalDuration": "00:00:00.0120000",
    "entries": {
        "database": {
            "data": {},
            "description": "The database did not answer.",
            "duration": "00:00:00.0100000",
            "exception": "System.TimeoutException: The operation has timed out.\n   at ...",
            "status": "Unhealthy",
            "tags": []
        }
    }
}
```

A check's `data` values are written with the metadata the HTTP JSON options resolve for them; a value no JSON context knows is written
as its `ToString()`. An entry's `exception` is the whole exception, its stack trace included, so serve the report on a port of its own
when it must not be public.

`LaunchPadHealthCheckResponseWriter.WriteAsync` is a `ResponseWriter` for a `MapHealthChecks` of your own too.

## A port of their own

Set `LaunchPad:HealthChecks:Port` and the health check endpoints move to that port:

```json
{
    "LaunchPad": {
        "HealthChecks": {
            "Port": 8081
        }
    }
}
```

or `LaunchPad__HealthChecks__Port=8081` in the environment. Then:

- Kestrel also listens on `http://*:8081`, next to the application's own addresses. Those come from `urls` (`ASPNETCORE_URLS`,
  `--urls`, `UseUrls`), else from `ASPNETCORE_HTTP_PORTS` and `ASPNETCORE_HTTPS_PORTS`, else Kestrel's default
  `http://localhost:5000`; LaunchPad adds the health port to them and writes the result back to `urls`. When the application declares
  `Kestrel:Endpoints`, which Kestrel prefers over `urls`, the port is added to Kestrel next to them.
- The health port serves the health check endpoints alone: anything else on it gets a 404, and so do the health check endpoints on any
  other port. This runs before any of the application's middleware.

The health port is plain HTTP.

The endpoints are kept apart only when the server really listens on the health port, which LaunchPad reads from the server's bound
addresses once it has started. When it doesn't, the health check endpoints answer on the application's own addresses, as they do with no
port. That happens when a `Listen` in code (`ConfigureKestrel`) overrides `urls`, under IIS or HTTP.sys, and on the in-memory test server,
so `WebApplicationFactory` tests of an application with a health port keep working.

## The options

`LaunchPadHealthCheckOptions` is bound from the `LaunchPad:HealthChecks` section by [Optionary](https://rocketsurgeonsguild.github.io/Optionary/),
without reflection. An application that references Optionary gets the section in its `appsettings.schema.json`, for completion and
validation in the editor. The options are validated with [ZeroAlloc.Validation](https://validation.zeroalloc.net) when the host starts, so a port that is not a
port number stops the application. Code
can change them with `services.Configure<LaunchPadHealthCheckOptions>(...)`.

| Option              | Default                                        | Meaning                                                                |
| ------------------- | ---------------------------------------------- | ---------------------------------------------------------------------- |
| `Port`              | none                                           | The port the endpoints are served on, alone.                           |
| `Path`              | `/health`                                      | The path of the health report.                                         |
| `LivePath`          | `live`                                         | The liveness path, under `Path`.                                       |
| `ReadyPath`         | `ready`                                        | The readiness path, under `Path`.                                      |
| `ResultStatusCodes` | `Healthy` 200, `Degraded` 200, `Unhealthy` 503 | The status code for each status, as `ResultStatusCodes:Degraded: 503`. |

## Kubernetes probes

```yaml
containers:
    - name: api
      ports:
          - name: http
            containerPort: 8080
          - name: health
            containerPort: 8081
      env:
          - name: LaunchPad__HealthChecks__Port
            value: '8081'
      livenessProbe:
          httpGet:
              path: /health/live
              port: health
      readinessProbe:
          httpGet:
              path: /health/ready
              port: health
      startupProbe:
          httpGet:
              path: /health/live
              port: health
          failureThreshold: 30
          periodSeconds: 2
```

The .NET container images set `ASPNETCORE_HTTP_PORTS=8080`, which LaunchPad keeps when it adds the health port. A `Service` that only
exposes `http` keeps the health endpoints inside the cluster.