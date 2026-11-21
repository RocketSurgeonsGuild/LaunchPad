// The samples, side by side. Each server sample is added by its project path (AddDotnetProject), and Aspire builds it before starting it.
// The samples' launch profiles all listen on ports 5000 and 5001, so the AppHost leaves them out and gives every sample its own endpoints
// and the environment the profiles would set (the AppHost's own, Development when it is run from here). Aspire sets the OTLP endpoint each
// one exports its logs, traces and metrics to.

#pragma warning disable ASPIREBLAZOR001 // the Blazor hosting integration is a preview

using System.Reflection;

using Aspire.Hosting.Dotnet;

var builder = DistributedApplication.CreateBuilder(args);

// ASP.NET Core with controllers, Swashbuckle's UI at /swagger
var restful = builder.AddSample("restful", "Sample.Restful").WithHttpHealthCheck("/health");
restful.WithUrl($"{restful.GetEndpoint("https")}/swagger", "Swagger UI");

var classicRestful = builder.AddSample("classic-restful", "Sample.Classic.Restful");
classicRestful.WithUrl($"{classicRestful.GetEndpoint("https")}/swagger", "Swagger UI");

// minimal APIs, Swashbuckle's UI at /swagger
var minimal = builder.AddSample("minimal", "Sample.Minimal").WithHttpHealthCheck("/health");
minimal.WithUrl($"{minimal.GetEndpoint("https")}/swagger", "Swagger UI");

// Hot Chocolate, Nitro at /graphql
var graphql = builder.AddSample("graphql", "Sample.Graphql");
graphql.WithUrl($"{graphql.GetEndpoint("https")}/graphql", "Nitro");

// gRPC: its Kestrel serves HTTP/2 only, so it gets the HTTPS endpoint alone
builder.AddSample("grpc", "Sample.Grpc", http: false);

builder.AddSample("pages", "Sample.Pages").WithHttpHealthCheck("/health");

builder.AddSample("blazor-server", "Sample.BlazorServer");

// a background service, no endpoints
builder.AddSample("worker", "Sample.Worker", http: false, https: false);

// a command line application that runs its command and exits, so it starts when asked to from the dashboard
builder.AddSample("command", "Sample.Command", http: false, https: false).WithExplicitStart();

// Blazor WebAssembly isn't a process of its own: the Blazor gateway serves the client to the browser. The integration builds the client with
// the AppHost's environment and has no option for its framework, so the AppHost names it there (Sample.BlazorWasm.csproj reads it).
Environment.SetEnvironmentVariable("LaunchPadSampleBrowserTargetFramework", $"{SampleResourceExtensions.TargetFramework}-browser");
var blazorWasm = builder.AddBlazorWasmApp("blazor-wasm", "../Sample.BlazorWasm/Sample.BlazorWasm.csproj");
builder.AddDotnetProjectBlazorGateway("blazor-wasm-gateway").WithBlazorClientApp(blazorWasm);

builder.Build().Run();

internal static class SampleResourceExtensions
{
    /// <summary>
    ///     The framework the samples run on (<c>LaunchPadLatestTargetFramework</c>, from the project file).
    /// </summary>
    public static readonly string TargetFramework = typeof(SampleResourceExtensions)
                                                    .Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                                                    .Single(static attribute => attribute.Key == "SampleTargetFramework")
                                                    .Value!;

    /// <summary>
    ///     Adds the sample in <c>sample/{project}/{project}.csproj</c> without its launch profile, with an HTTP and an HTTPS endpoint on ports
    ///     Aspire chooses and the AppHost's environment.
    /// </summary>
    /// <remarks>
    ///     The samples set <c>TargetFrameworks</c> (every framework LaunchPad supports), so building or running one needs a framework, which
    ///     <c>AddDotnetProject</c> has no option for. It is given to the build as the <c>TargetFramework</c> environment variable, which MSBuild
    ///     reads as a property; the projects that set a <c>TargetFramework</c> of their own (the generators, on netstandard2.0) keep theirs.
    /// </remarks>
    public static IResourceBuilder<DotnetProjectResource> AddSample(
        this IDistributedApplicationBuilder builder,
        string name,
        string project,
        bool http = true,
        bool https = true
    )
    {
        var sample = builder
                    .AddDotnetProject(name, $"../{project}/{project}.csproj", static options => options.ExcludeLaunchProfile = true)
                    .WithBuildEnvironment("TargetFramework", TargetFramework)
                    .WithEnvironment("DOTNET_ENVIRONMENT", builder.Environment.EnvironmentName)
                    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);
        if (http)
        {
            sample.WithHttpEndpoint();
        }

        if (https)
        {
            sample.WithHttpsEndpoint();
        }

        return sample;
    }
}
