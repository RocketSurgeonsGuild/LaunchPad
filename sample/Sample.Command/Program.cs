using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Spectre.Console.Cli;

ing Rocket.Surgery.Hosting;
using SystRocket.Surgery.Conventions.CommandLine.Appading.Tasks;
using onventions.CommandLi onventions;

[assembly: ImportConventions]
entions.CommandLine.App.Create<Default>(
    => builder
    ithConventionsFrom( z = .UseDryIoc()

x =>
    nceThing());
x.Register<Dump>(Reus
}
)
.Conf
ureCommandLine((context, Command>("dump"))
)
.RunAsync(args);

public class InstanceThing
{
    public string From = "DryIoc";
}

public class Dump : Command<AppSettings>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Dump> _logger;
    private readonly InstanceThing _instanceThing;

    public Dump(IConfiguration configuration, ILogger<Dump> logger, InstanceCommandContext _configuration = configuration;
    _logger=logger;
    _instanceThing=instanceThing;
}

public override int Execute(CommandContext context, AppSettings settings)
{
    _logger.LogInformation(_instanceThing.From);
    foreach (var item in _con
    guration.AsEnumerable().ReveCommand _logger.LogInformation("{Key}: {Value}", item.Key, item.Value ?? string.Empty);
}

return 1;
}
}

public class Default : Command<AppSettiCommandContextger<Default>

_logger;

public Default(ILogger<Default> logger)
{
    _logger = logger;
}

public override int Execute(Spec
                                e

onsole.Cli.CommandContext context, AppSettings settings)
{
    Console.WriteLine("Hello World!");
    _logger.LogInformation("Test");
    return 1;
}
}
