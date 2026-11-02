using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     A project that references a gRPC server package itself gets <c>Rocket.Surgery.LaunchPad.Grpc</c>. <c>LaunchPadEnableGrpc</c> set
///     to <c>false</c> opts a project out.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkGrpcTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     A gRPC server, by either of the packages an application references for it, gets LaunchPad's gRPC package.
    /// </summary>
    /// <param name="package">The gRPC package the application references.</param>
    [Test]
    [Arguments("Grpc.AspNetCore")]
    [Arguments("Grpc.AspNetCore.Server")]
    public async Task Should_Add_LaunchPad_Grpc_For_A_Direct_Grpc_Server_Reference(string package)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, HostModelShape.Project(HostModel.AspNetCore, targetFramework).ItemPackageReference(package));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A gRPC client is not a gRPC server: LaunchPad's package holds the interceptors of a service, so a client gets nothing.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_LaunchPad_Grpc_For_A_Grpc_Client()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemPackageReference("Grpc.Net.Client"));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     <c>LaunchPadEnableGrpc</c> set to <c>false</c> leaves the package out, and the FluentValidation integration with it.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_The_Grpc_Packages_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            HostModelShape
               .Project(HostModel.AspNetCore, targetFramework)
               .Property("LaunchPadEnableGrpc", "false")
               .ItemPackageReference("Grpc.AspNetCore")
               .ItemPackageReference("FluentValidation")
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }
}
