using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Surgery.Conventions.CommandLine;
using Sample.Command.Conventions;

[assembly: ImportConventions]

await Rocket.Surgery.Conventions.CommandLine.App.Create<Default>(
                 builder => builder
                           .WithConventionsFrom(Imports.GetConventions)
                           .ConfigureLogging(z => z.AddConsole())
                           .UseDryIoc()
                           .ConfigureDryIoc(
                                x =>
                                {
                                    x.UseInstance(new InstanceThing());
                                    x.Register<Dump>(Reuse.Singleton);
                                }
                            )
                           .ConfigureCommandLine((context, app) => app.AddCommand<Dump>("dump"))
             )
            .RunAsync(args);

public class InstanceThing
{
    public string From = "DryIoc";
}

public class Dump : Spectre.Console.Cli.Command<AppSettings>
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

    public override int Execute(Spectre.Console.Cli.CommandContext context, AppSettings settings)
    {
        _logger.LogInformation(_instanceThing.From);
        foreach (var item in _configuration.AsEnumerable().Reverse())
        {
            _logger.LogInformation("{Key}: {Value}", item.Key, item.Value ?? string.Empty);
        }

        return 1;
    }
}

public class Default : Spectre.Console.Cli.Command<AppSettings>
{
    private readonly ILogger<Default> _logger;

    public Default(ILogger<Default> logger)
    {
        _logger = logger;
    }

    public override int Execute(Spectre.Console.Cli.CommandContext context, AppSettings settings)
    {
        Console.WriteLine("Hello World!");
        _logger.LogInformation("Test");
        return 1;
    }
}
