using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     <c>Rocket.Surgery.LaunchPad.Sdk.Console</c>: the LaunchPad SDK for console applications, which chains Joinery.Dovetail.Sdk.Console
///     (ConsoleAppFramework) instead of Joinery.Dovetail.Sdk and adds the same LaunchPad packages.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway app.</param>
[TargetFrameworks]
public class SdkConsoleTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     A project that names the console SDK alone is an Exe on the generic host and gets ConsoleAppFramework and LaunchPad.Hosting. Its
    ///     commands, registered by an <c>IConsoleAppJoint</c>, are created from the host's services, and Dovetail writes the entry point.
    /// </summary>
    [Test]
    public async Task Should_Build_And_Run_A_Console_App_That_Names_The_Sdk_Alone()
    {
        var packages = await PackedSdk.Pack();
        using var tree = new SdkTree(packages, targetFramework);
        // a Directory.Build.props of its own, so the tree's, which applies Rocket.Surgery.LaunchPad.Sdk to every project below it, doesn't reach the project
        tree
           .AddFile("consoleapp/Directory.Build.props", ProjectCreator.Create().Property("NuGetAudit", "false"))
           .AddFile(
                "consoleapp/consoleapp.csproj",
                ProjectCreator.Templates.SdkCsproj(sdk: $"{PackedSdk.ConsoleSdkPackageId}/{packages.Version}", targetFramework: targetFramework)
            );
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "consoleapp", "Commands.cs"), Commands, Cancellation);

        var evaluation = await tree.Evaluate("consoleapp", Cancellation);
        await tree.Build("consoleapp", Cancellation);
        var output = await tree.Run("consoleapp", Cancellation, "greet --name LaunchPad");

        await evaluation.Hosts.Should().Contain("HostApplication");
        await evaluation.PackageReferences.Should().Contain($"Rocket.Surgery.LaunchPad.Hosting {{LaunchPadVersion}} (SDK)");
        await output.Should().Contain("Hello, LaunchPad, from a joint");
    }

    /// <summary>
    ///     The console SDK is MSBuild files only: its own Sdk.props and Sdk.targets, and the files it shares with
    ///     <c>Rocket.Surgery.LaunchPad.Sdk</c>.
    /// </summary>
    [Test]
    public async Task Should_Ship_Only_Msbuild_Files()
    {
        var packages = await PackedSdk.Pack();

        await Verify(packages.ConsoleSdkFiles);
    }

    /// <summary>
    ///     The LaunchPad packages are added at the version of the SDK, and Dovetail's console SDK is chained at the version the repository
    ///     pins.
    /// </summary>
    [Test]
    public async Task Should_Generate_The_Package_Versions_When_Packed()
    {
        var packages = await PackedSdk.Pack();

        await Verify(packages.Scrub(packages.ConsolePackagesProps), "xml");
    }

    /// <summary>
    ///     A command class built from the host's services, and the joints that register the service and the command.
    /// </summary>
    private const string Commands = """
        using System;
        using ConsoleAppFramework;
        using Joinery.Dovetail;
        using Joinery.Dovetail.Attributes;
        using Joinery.Dovetail.Joints;
        using Microsoft.Extensions.DependencyInjection;

        public class Greeter
        {
            public string Greet(string name) => $"Hello, {name}, from a joint";
        }

        internal class GreetCommands(Greeter greeter)
        {
            [Command("greet")]
            public void Greet(string name = "world") => Console.WriteLine(greeter.Greet(name));
        }

        [DovetailExport]
        internal class AppJoint : IServiceJoint, IConsoleAppJoint
        {
            public void Register(IDovetailContext context, IServiceCollection services) => services.AddSingleton<Greeter>();

            public void Register(IDovetailContext context, ConsoleApp.ConsoleAppBuilder app) => app.Add<GreetCommands>();
        }
        """;
}
