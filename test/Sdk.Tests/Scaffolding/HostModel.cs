using Microsoft.Build.Utilities.ProjectCreation;

namespace Sdk.Tests.Scaffolding;

/// <summary>
///     The host models a throwaway project can be. Each is declared the way an application declares itself, by its project SDK, its output
///     type and its references, and none declares a <c>DovetailHost</c>: finding the host model is the SDK's job.
/// </summary>
public enum HostModel
{
    /// <summary>
    ///     No host: a library.
    /// </summary>
    None,

    /// <summary>
    ///     The generic host: an executable that references <c>Microsoft.Extensions.Hosting</c>.
    /// </summary>
    GenericHost,

    /// <summary>
    ///     ASP.NET Core: a <c>Microsoft.NET.Sdk.Web</c> project.
    /// </summary>
    AspNetCore,

    /// <summary>
    ///     Blazor WebAssembly: a <c>Microsoft.NET.Sdk.BlazorWebAssembly</c> project.
    /// </summary>
    WebAssembly,
}

/// <summary>
///     What a throwaway project of each host model is made of.
/// </summary>
/// <param name="Sdk">The project SDK.</param>
/// <param name="OutputType">The output type, when the project SDK does not imply it.</param>
/// <param name="Packages">The packages an application of the host model references itself.</param>
/// <param name="Program">What its <c>Program.cs</c> holds.</param>
internal sealed record HostModelShape(string Sdk, string? OutputType, IReadOnlyList<string> Packages, string Program)
{
    public const string HealthChecks = "Microsoft.Extensions.Diagnostics.HealthChecks";

    /// <summary>
    ///     The namespace of the generated <c>ConfigureDovetail</c> entry points, which <see cref="Program" /> imports.
    /// </summary>
    public const string RootNamespace = "Throwaway";

    /// <summary>
    ///     The shape of a host model.
    /// </summary>
    /// <param name="host">The host model.</param>
    /// <returns>The shape.</returns>
    public static HostModelShape Of(HostModel host) =>
        host switch
        {
            // a library has nothing to run
            HostModel.None => new("Microsoft.NET.Sdk", null, [], "public static class Library;"),
            HostModel.GenericHost => new(
                "Microsoft.NET.Sdk",
                "Exe",
                ["Microsoft.Extensions.Hosting"],
                ReportHealthChecks("Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args)")
            ),
            HostModel.AspNetCore => new("Microsoft.NET.Sdk.Web", null, [], ReportHealthChecks("Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args)")),
            // a WebAssembly host cannot run on the machine the tests do
            HostModel.WebAssembly => new(
                "Microsoft.NET.Sdk.BlazorWebAssembly",
                null,
                ["Microsoft.AspNetCore.Components.WebAssembly"],
                """System.Console.WriteLine("hello");"""
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(host), host, null),
        };

    /// <summary>
    ///     A throwaway project of a host model, which references the packages of the host model.
    /// </summary>
    /// <param name="host">The host model.</param>
    /// <param name="targetFramework">The target framework.</param>
    /// <param name="centralVersions">
    ///     Whether the versions are left to <c>Directory.Packages.props</c> (central package management); otherwise each reference names the
    ///     version the repository pins.
    /// </param>
    /// <returns>The project, to add to.</returns>
    public static ProjectCreator Project(HostModel host, string targetFramework, bool centralVersions = false)
    {
        var shape = Of(host);
        var project = ProjectCreator
                     .Templates.SdkCsproj(sdk: shape.Sdk, targetFramework: targetFramework, outputType: shape.OutputType)
                     .Property("RootNamespace", RootNamespace);
        foreach (var package in shape.Packages)
        {
            project.ItemPackageReference(package, centralVersions ? null : Repository.PackageVersion(package, targetFramework));
        }

        return project;
    }

    // The generated ConfigureDovetail entry point builds the host with the joints Dovetail discovered; whether LaunchPad.Hosting's health
    // check joint is among them is what the application reports. It looks the type up by name, as the application may not reference it.
    private static string ReportHealthChecks(string builder) => $$"""
        using {{RootNamespace}};

        var host = await {{builder}}.ConfigureDovetail();
        System.Console.WriteLine($"HealthChecks:{Registered(host.Services)}");

        static bool Registered(System.IServiceProvider services) =>
            System.Type.GetType("Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService, Microsoft.Extensions.Diagnostics.HealthChecks") is { } service
         && services.GetService(service) is not null;
        """;
}
