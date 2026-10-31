using Microsoft.Extensions.Configuration;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Sample.Command;

/// <summary>
///     The settings of the commands. The sample has no options.
/// </summary>
public class AppSettings : CommandSettings;

/// <summary>
///     Something that only the DryIoc container knows about.
/// </summary>
public class InstanceThing
{
    public string From => "DryIoc";
}

/// <summary>
///     Writes the configuration the host was started with.
/// </summary>
public class Dump(IConfiguration configuration, IAnsiConsole console, InstanceThing instanceThing) : Command<AppSettings>
{
    public override int Execute(CommandContext context, AppSettings settings, CancellationToken cancellationToken)
    {
        console.WriteLine($"From: {instanceThing.From}");
        foreach (var item in configuration.AsEnumerable().Reverse())
        {
            console.WriteLine($"{item.Key}: {item.Value ?? ""}");
        }

        return 0;
    }
}

/// <summary>
///     Runs when no command is given.
/// </summary>
public class DefaultCommand(IAnsiConsole console) : Command<AppSettings>
{
    public override int Execute(CommandContext context, AppSettings settings, CancellationToken cancellationToken)
    {
        console.WriteLine("Hello World!");
        return 0;
    }
}
