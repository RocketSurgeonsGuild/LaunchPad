using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.CommandLine;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Sample.Command;
using Spectre.Console.Cli;

var builder = Host.CreateApplicationBuilder(args);

var host = await builder.LaunchWith(
    RocketBooster.For(Imports.Instance),
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

public class InstanceThing
{
    public string From => "DryIoc";
}


public class Dump(IConfiguration configuration, ILogger<Dump> logger, InstanceThing instanceThing)
    : Command<AppSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] AppSettings settings)
    {
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        logger.LogInformation(instanceThing.From);
        foreach (var item in configuration.AsEnumerable().Reverse())
        {
            logger.LogInformation("{Key}: {Value}", item.Key, item.Value ?? string.Empty);
        }

        return 1;
    }
}

public class DefaultCommand(ILogger<DefaultCommand> logger) : Command<AppSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] AppSettings settings)
    {
        Console.WriteLine("Hello World!");
        logger.LogInformation("Test");
        return 1;
    }
}