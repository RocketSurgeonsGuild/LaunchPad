using ConsoleAppFramework;

using Joinery.Dovetail;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sample.Command;

/// <summary>
///     The entry point, written by hand (<c>DovetailUseDefaultProgram=false</c>) so the tests can run the application in-process. It does
///     what the entry point Dovetail writes for a console application does: builds the generic host with Dovetail, applies the
///     <c>ConsoleApp</c> joints and runs the command the command line names.
/// </summary>
public static class Program
{
    /// <summary>
    ///     Runs the application.
    /// </summary>
    /// <param name="args">The command line.</param>
    /// <returns>The exit code.</returns>
    public static Task<int> Main(string[] args) => RunAsync(args);

    /// <summary>
    ///     Starts the host and runs the command given in <paramref name="args" />.
    /// </summary>
    /// <param name="args">The command line arguments. No arguments run the root command, which greets.</param>
    /// <param name="output">Where the commands write. The default is the standard output.</param>
    /// <param name="configure">Changes the host builder before the host is created, for example to add configuration.</param>
    /// <param name="cancellationToken">Cancels the host start and the command.</param>
    /// <returns>The exit code of the command.</returns>
    /// <remarks>
    ///     ConsoleAppFramework keeps the service provider and the exit code in statics (<see cref="ConsoleApp.ServiceProvider" /> and
    ///     <see cref="Environment.ExitCode" />), so only one run can be in progress at a time.
    /// </remarks>
    public static async Task<int> RunAsync(
        string[] args,
        TextWriter? output = null,
        Action<HostApplicationBuilder>? configure = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(args);

        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSingleton(new CommandOutput(output ?? Console.Out));
        configure?.Invoke(builder);

        // the joints that register commands run once the host is built, with the context the host was configured with
        IDovetailContext? dovetail = null;
        using var host = await builder.ConfigureDovetail(
            context => context.ConfigureHostCreated<IHost>((created, _) => dovetail = created),
            cancellationToken
        );

        ConsoleApp.ServiceProvider = host.Services;
        var app = ConsoleApp.Create();
        await app.ApplyConsoleApp(dovetail!, cancellationToken);

        Environment.ExitCode = 0;
        await host.StartAsync(cancellationToken);
        try
        {
            await app.RunAsync(args, startHost: false, stopHost: false, disposeServiceProvider: false, cancellationToken);
        }
        finally
        {
            await host.StopAsync(CancellationToken.None);
        }

        return Environment.ExitCode;
    }
}
