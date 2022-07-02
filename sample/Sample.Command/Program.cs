using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Sample.Command;
using Spectre.Console.Cli;

[assembly: ImportConventions(Namespace = "Sample.Command")]

await Rocket.Surgery.Conventions.CommandLine.App.Create<DefaultCommand>(
                 builder => builder
                           .WithConventionsFrom(Imports.GetConventions)
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
             )
            .RunAsync(args);


public class InstanceThing
{
    public string From => "DryIoc";
}


public class Dump : Command<AppSettings>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Dump> _logger;
    private readonly InstanceThing _instanceThing;

    public Dump(IConfiguration configuration, ILogger<Dump> logger, InstanceThing instanceThing)
    {
        _configuration = configuration;
        _logger = logger;
        _instanceThing = instanceThing;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] AppSettings settings)
    {
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        _logger.LogInformation(_instanceThing.From);
        foreach (var item in _configuration.AsEnumerable().Reverse())
        {
            _logger.LogInformation("{Key}: {Value}", item.Key, item.Value ?? string.Empty);
        }

        return 1;
    }
}

public class DefaultCommand : Command<AppSettings>
{
    private readonly ILogger<DefaultCommand> _logger;

    public DefaultCommand(ILogger<DefaultCommand> logger)
    {
        _logger = logger;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] AppSettings settings)
    {
        Console.WriteLine("Hello World!");
        _logger.LogInformation("Test");
        return 1;
    }
}
