// The JavaScript initializer of the application (Blazor imports {assembly}.lib.module.js before it starts the runtime). Sample.AppHost's
// Blazor gateway serves the client's settings at _blazor/_configuration, under the client's path: its OpenTelemetry settings
// (OTEL_EXPORTER_OTLP_ENDPOINT, the gateway's own OTLP route, OTEL_EXPORTER_OTLP_PROTOCOL and OTEL_SERVICE_NAME) and the services it
// references. They become the runtime's environment variables, which LaunchPad.Hosting's browser exporter joint reads. Served
// anywhere else (dotnet run, the DevServer) there are no settings, and the application exports nothing.
export async function onRuntimeConfigLoaded(config) {
    try {
        const response = await fetch(new URL("_blazor/_configuration", document.baseURI));
        // the DevServer answers an unknown path with index.html
        if (!response.ok || !response.headers.get("content-type")?.includes("json")) {
            return;
        }

        const environment = (await response.json())?.webAssembly?.environment;
        if (environment) {
            config.environmentVariables ??= {};
            Object.assign(config.environmentVariables, environment);
        }
    } catch (error) {
        console.warn("The client settings could not be loaded:", error);
    }
}
