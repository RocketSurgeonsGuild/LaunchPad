using System.Text;

namespace Hosting.Tests.Scaffolding;

/// <summary>
///     The host models a throwaway project can be, declared the way the samples declare them (the declaration the Dovetail documentation
///     shows for a project that does not use <c>Joinery.Dovetail.Sdk</c>; RocketSurgeonsGuild/LaunchPad#3072 replaces it with SDK detection).
/// </summary>
public enum HostModel
{
    /// <summary>
    ///     No host: a library, which has nothing to configure.
    /// </summary>
    None,

    /// <summary>
    ///     The generic host, <c>Microsoft.Extensions.Hosting</c> (<c>HostApplication</c>).
    /// </summary>
    GenericHost,

    /// <summary>
    ///     ASP.NET Core (<c>WebApplication</c>).
    /// </summary>
    AspNetCore,

    /// <summary>
    ///     Blazor WebAssembly (<c>WebAssembly</c>).
    /// </summary>
    WebAssembly,
}

/// <summary>
///     A csproj written for a throwaway project.
/// </summary>
internal sealed class AppProject
{
    public const string HealthChecks = "Microsoft.Extensions.Diagnostics.HealthChecks";

    private const string RootNamespace = "Throwaway";

    private AppProject(HostModel host, bool versioned)
    {
        _host = host;
        _versioned = versioned;
        if (host == HostModel.None) return;

        AddPackage("Joinery.Dovetail.Hosting");
        switch (host)
        {
            case HostModel.GenericHost:
                AddPackage("Microsoft.Extensions.Hosting");
                break;
            case HostModel.WebAssembly:
                AddPackage("Microsoft.AspNetCore.Components.WebAssembly");
                break;
            case HostModel.None:
                break;
            case HostModel.AspNetCore:
                break;
            default:
                break;
        }
    }

    /// <summary>
    ///     A project that gets its package versions from <c>Directory.Packages.props</c> in the repository it lives in.
    /// </summary>
    /// <param name="host">The host model.</param>
    /// <returns>The project.</returns>
    public static AppProject Central(HostModel host) => new(host, false);

    /// <summary>
    ///     A project of its own: it names the versions of its packages (which are those of <c>Directory.Packages.props</c>).
    /// </summary>
    /// <param name="host">The host model.</param>
    /// <returns>The project.</returns>
    public static AppProject Standalone(HostModel host) => new(host, true);

    /// <summary>
    ///     References a package.
    /// </summary>
    /// <param name="packageId">The package id.</param>
    /// <param name="version">The version; the one the repository pins for projects that name their versions.</param>
    /// <returns>This project.</returns>
    public AppProject AddPackage(string packageId, string? version = null)
    {
        _packages.Add((packageId, version));
        return this;
    }

    /// <summary>
    ///     References a project.
    /// </summary>
    /// <param name="path">The path of the project file.</param>
    /// <returns>This project.</returns>
    public AppProject AddProjectReference(string path)
    {
        _projectReferences.Add(path);
        return this;
    }

    /// <summary>
    ///     Replaces what the application's <c>Program.cs</c> holds.
    /// </summary>
    /// <param name="program">The text of <c>Program.cs</c>.</param>
    /// <returns>This project.</returns>
    public AppProject WithProgram(string program)
    {
        _program = program;
        return this;
    }

    /// <summary>
    ///     Sets an MSBuild property in the project.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value.</param>
    /// <returns>This project.</returns>
    public AppProject WithProperty(string name, string value)
    {
        _properties.Add((name, value));
        return this;
    }

    /// <summary>
    ///     What the application's <c>Program.cs</c> holds.
    /// </summary>
    public string Program =>
        _program ?? _host switch
        {
            // The generated ConfigureDovetail entry point builds the host with the joints Dovetail discovered; whether the health checks
            // are among them is what the application reports. It looks the type up by name, as the application may not reference it.
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
            _ => """System.Console.WriteLine("hello");""",
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
    /// <returns>The csproj.</returns>
    public string ToXml(string targetFramework)
    {
        var xml = new StringBuilder();
        xml.AppendLine($"""<Project Sdk="{( _host == HostModel.AspNetCore ? "Microsoft.NET.Sdk.Web" : "Microsoft.NET.Sdk" )}">""");
        xml.AppendLine("  <PropertyGroup>");
        xml.AppendLine($"    <TargetFramework>{targetFramework}</TargetFramework>");
        // the namespace of the generated ConfigureDovetail entry points, which Program.cs imports
        xml.AppendLine($"    <RootNamespace>{RootNamespace}</RootNamespace>");
        if (_host != HostModel.None) xml.AppendLine("    <OutputType>Exe</OutputType>");
        foreach (var (name, value) in _properties)
        {
            xml.AppendLine($"    <{name}>{value}</{name}>");
        }

        xml.AppendLine("  </PropertyGroup>");
        xml.AppendLine("  <ItemGroup>");
        foreach (var (id, version) in _packages)
        {
            xml.AppendLine(
                _versioned ? $"""    <PackageReference Include="{id}" Version="{version ?? Repository.PackageVersion(id)}" />""" : $"""    <PackageReference Include="{id}" />"""
            );
        }

        foreach (var path in _projectReferences)
        {
            xml.AppendLine($"""    <ProjectReference Include="{path}" />""");
        }

        xml.AppendLine("  </ItemGroup>");
        if (HostDeclaration is { } declaration)
        {
            xml.AppendLine("  <ItemGroup>");
            xml.AppendLine(declaration);
            xml.AppendLine("  </ItemGroup>");
        }

        xml.AppendLine("</Project>");
        return xml.ToString();
    }

    private string? HostDeclaration =>
        _host switch
        {
            HostModel.GenericHost => """
                    <DovetailHost
                      Include="HostApplication"
                      HostBuilderType="global::Microsoft.Extensions.Hosting.HostApplicationBuilder"
                      HostBuilderName="builder"
                      HostType="global::Microsoft.Extensions.Hosting.IHost"
                      HelperName="global::Joinery.Dovetail.DovetailHostApplicationHelpers" />
                """,
            HostModel.AspNetCore => """
                    <DovetailHost
                      Include="WebApplication"
                      HostBuilderType="global::Microsoft.AspNetCore.Builder.WebApplicationBuilder"
                      HostBuilderName="builder"
                      HostType="global::Microsoft.AspNetCore.Builder.WebApplication"
                      HelperName="global::Joinery.Dovetail.DovetailHostApplicationHelpers" />
                """,
            HostModel.WebAssembly => """
                    <DovetailHost
                      Include="WebAssembly"
                      HostBuilderType="global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder"
                      HostBuilderName="builder"
                      HostType="global::Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHost" />
                """,
            _ => null,
        };

    private readonly HostModel _host;
    private readonly List<(string Id, string? Version)> _packages = [];
    private readonly List<string> _projectReferences = [];
    private readonly List<(string Name, string Value)> _properties = [];
    private string? _program;
    private readonly bool _versioned;
}
