using DryIoc;

using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Sample.Command;

/// <summary>
///     The command line sample: a plain generic host started through Dovetail's generated <c>ConfigureDovetail</c>, with the commands
///     of Spectre.Console.Cli created by the DryIoc container that Joinery.Dovetail.DryIoc builds the host's service provider from.
/// </summary>
public static class CommandLineApplication
{
    /// <summary>
    ///     Starts the host and runs the command given in <paramref name="args" />.
    /// </summary>
    /// <param name="args">The command line arguments. No arguments run <see cref="DefaultCommand" />.</param>
    /// <param name="console">The console the commands write to. The default is the standard console.</param>
    /// <param name="configure">Changes the host builder before the host is created, for example to add configuration.</param>
    /// <param name="cancellationToken">Cancels the host start and the command.</param>
    /// <returns>The exit code of the command.</returns>
    public static async Task<int> RunAsync(
        string[] args,
        IAnsiConsole? console = null,
        Action<HostApplicationBuilder>? configure = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(args);
        console ??= AnsiConsole.Console;

        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSingleton(console);
        configure?.Invoke(builder);

        using var host = await builder.ConfigureDovetail(
            context => context.ConfigureDryIoc(
                (_, container) =>
                {
                    container.Use(new InstanceThing());
                    container.Register<Dump>(Reuse.Singleton);
                }
            ),
            cancellationToken
        );

        var app = new CommandApp<DefaultCommand>(new HostTypeRegistrar(host.Services));
        app.Configure(
            configurator =>
            {
                configurator.Settings.Console = console;
                configurator.AddCommand<Dump>("dump").WithDescription("Writes the configuration the host was started with");
            }
        );
        return await app.RunAsync(args, cancellationToken);
    }
}
