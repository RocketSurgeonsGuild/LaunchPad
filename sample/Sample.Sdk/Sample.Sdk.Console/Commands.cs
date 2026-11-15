using ConsoleAppFramework;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Sample.Sdk.Console;

/// <summary>
///     The commands. ConsoleAppFramework creates the class from the host's services, which hold LaunchPad.Hosting's health checks.
/// </summary>
internal sealed class HealthCommands(HealthCheckService healthChecks)
{
    /// <summary>
    ///     Writes the health of the application. It is the root command, so it runs when no command is given.
    /// </summary>
    [Command("")]
    public async Task Health(CancellationToken cancellationToken)
    {
        var report = await healthChecks.CheckHealthAsync(cancellationToken);
        System.Console.WriteLine($"Health: {report.Status}");
    }
}

/// <summary>
///     Registers the commands. ConsoleAppFramework reads the <c>Add</c> calls in the application's own compilation, so this joint has to be
///     in the application.
/// </summary>
[DovetailExport]
internal sealed class CommandsJoint : IConsoleAppJoint
{
    public void Register(IDovetailContext context, ConsoleApp.ConsoleAppBuilder app) => app.Add<HealthCommands>();
}
