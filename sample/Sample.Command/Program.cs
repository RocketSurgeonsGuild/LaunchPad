using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.CommandLine;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Spectre.Console.Cli;

var builder = Host.CreateApplicationBuilder(args);

var host = await builder.ConfigureRocketSurgery(
    b => b
        .SetDefaultCommand<DefaultCommand>()
        .ConfigureLogging(z => z.AddConsole())
        .UseDryIoc()
        .ConfigureDryIoc(
             x =>
             {
                 x.Use(new InstanceThing());
                 x.Register<Dump>(Reuse.Singleton);
             }
         )
        .ConfigureCommandLine((_, app) => app.AddCommand<Dump>("dump"))
);
await host.RunAsync();

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public class InstanceThing
{
    public string From => "DryIoc";

    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            return ToString();
        }
    }
}

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Dump(IConfiguration configuration, ILogger<Dump> logger, InstanceThing instanceThing)
    : Command<AppSettings>
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            return ToString();
        }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] AppSettings settings)
    {
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        logger.LogInformation(instanceThing.From);
        foreach (var item in configuration.AsEnumerable().Reverse())
        {
            logger.LogInformation("{Key}: {Value}", item.Key, item.Value ?? "");
        }

        return 1;
    }
}

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DefaultCommand(ILogger<DefaultCommand> logger) : Command<AppSettings>
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            return ToString();
        }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] AppSettings settings)
    {
        Console.WriteLine("Hello World!");
        logger.LogInformation("Test");
        return 1;
    }
}
