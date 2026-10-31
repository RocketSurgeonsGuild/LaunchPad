using System.Text;

using Hosting.Tests.Scaffolding;

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
///     A csproj written for a throwaway project that gets its LaunchPad wiring from the SDK applied in the tree's
///     <c>Directory.Build.props</c>. It names the version of each package it references (the one <c>Directory.Packages.props</c> pins), as
///     a project of its own does.
/// </summary>
internal sealed class SdkApp
{
    public const string HealthChecks = "Microsoft.Extensions.Diagnostics.HealthChecks";

    private const string RootNamespace = "Throwaway";

    public SdkApp(HostModel host)
    {
        Host = host;
        switch (host)
        {
            case HostModel.GenericHost:
                AddPackage("Microsoft.Extensions.Hosting");
                break;
            case HostModel.WebAssembly:
                AddPackage("Microsoft.AspNetCore.Components.WebAssembly");
                break;
            case HostModel.None:
            case HostModel.AspNetCore:
            default:
                break;
        }
    }

    /// <summary>
    ///     The host model.
    /// </summary>
    public HostModel Host { get; }

    /// <summary>
    ///     References a package.
    /// </summary>
    /// <param name="packageId">The package id.</param>
    /// <param name="version">The version; the one the repository pins when it is left out.</param>
    /// <returns>This project.</returns>
    public SdkApp AddPackage(string packageId, string? version = null)
    {
        _packages.Add((packageId, version));
        return this;
    }

    /// <summary>
    ///     References a project of the tree.
    /// </summary>
    /// <param name="name">The name of the project (and of its folder).</param>
    /// <returns>This project.</returns>
    public SdkApp AddProjectReference(string name)
    {
        _projectReferences.Add($"../{name}/{name}.csproj");
        return this;
    }

    /// <summary>
    ///     Sets a property in the project file.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The value.</param>
    /// <returns>This project.</returns>
    public SdkApp SetProperty(string name, string value)
    {
        _properties.Add((name, value));
        return this;
    }

    /// <summary>
    ///     The packages the project references, with their versions.
    /// </summary>
    public IEnumerable<(string Id, string Version)> Packages => _packages.Select(z => (z.Id, z.Version ?? Repository.PackageVersion(z.Id)));

    /// <summary>
    ///     What the application's <c>Program.cs</c> holds.
    /// </summary>
    public string Program =>
        Host switch
        {
            // The generated ConfigureDovetail entry point builds the host with the joints Dovetail discovered; whether LaunchPad.Hosting's
            // health check joint is among them is what the application reports. It looks the type up by name, as the application may not
            // reference it.
            HostModel.GenericHost => """
                using Throwaway;

                var host = await Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args).ConfigureDovetail();
                System.Console.WriteLine($"HealthChecks:{Registered(host.Services)}");
                """ + RegisteredFunction,
            HostModel.AspNetCore => """
                using Throwaway;

                var app = await Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args).ConfigureDovetail();
                System.Console.WriteLine($"HealthChecks:{Registered(app.Services)}");
                """ + RegisteredFunction,
            // a library has nothing to run
            HostModel.None => "public static class Library;",
            // a WebAssembly host cannot run on the machine the tests do
            HostModel.WebAssembly => """System.Console.WriteLine("hello");""",
            _ => throw new ArgumentOutOfRangeException(nameof(Host), Host, null),
        };

    private const string RegisteredFunction = """


        static bool Registered(System.IServiceProvider services) =>
            System.Type.GetType("Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService, Microsoft.Extensions.Diagnostics.HealthChecks") is { } service
         && services.GetService(service) is not null;
        """;

    /// <summary>
    ///     Writes the csproj.
    /// </summary>
    /// <param name="targetFramework">The target framework.</param>
    /// <param name="centralVersions">Whether the versions are left to <c>Directory.Packages.props</c> (central package management).</param>
    /// <returns>The csproj.</returns>
    public string ToXml(string targetFramework, bool centralVersions = false)
    {
        var sdk = Host switch
        {
            HostModel.None or HostModel.GenericHost => "Microsoft.NET.Sdk",
            HostModel.AspNetCore => "Microsoft.NET.Sdk.Web",
            HostModel.WebAssembly => "Microsoft.NET.Sdk.BlazorWebAssembly",
            _ => throw new ArgumentOutOfRangeException(nameof(Host), Host, null),
        };
        var xml = new StringBuilder();
        xml.AppendLine($"""<Project Sdk="{sdk}">""");
        xml.AppendLine("  <PropertyGroup>");
        xml.AppendLine($"    <TargetFramework>{targetFramework}</TargetFramework>");
        // the namespace of the generated ConfigureDovetail entry points, which Program.cs imports
        xml.AppendLine($"    <RootNamespace>{RootNamespace}</RootNamespace>");
        if (Host == HostModel.GenericHost) xml.AppendLine("    <OutputType>Exe</OutputType>");
        foreach (var (name, value) in _properties)
        {
            xml.AppendLine($"    <{name}>{value}</{name}>");
        }

        xml.AppendLine("  </PropertyGroup>");
        xml.AppendLine("  <ItemGroup>");
        foreach (var (id, version) in Packages)
        {
            xml.AppendLine(centralVersions ? $"""    <PackageReference Include="{id}" />""" : $"""    <PackageReference Include="{id}" Version="{version}" />""");
        }

        foreach (var path in _projectReferences)
        {
            xml.AppendLine($"""    <ProjectReference Include="{path}" />""");
        }

        xml.AppendLine("  </ItemGroup>");
        xml.AppendLine("</Project>");
        return xml.ToString();
    }

    private readonly List<(string Id, string? Version)> _packages = [];
    private readonly List<(string Name, string Value)> _properties = [];
    private readonly List<string> _projectReferences = [];
}
