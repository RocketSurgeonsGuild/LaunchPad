using ConsoleAppFramework;

using Microsoft.Extensions.Configuration;

namespace Sample.Command;

/// <summary>
///     Something that only the DryIoc container knows about: <see cref="DryIocSetup" /> registers it there, not in the service collection.
/// </summary>
public class InstanceThing
{
    public string From => "DryIoc";
}

/// <summary>
///     Where the commands write. The entry point registers the standard output, or the writer a test passes.
/// </summary>
/// <param name="Writer">The writer.</param>
public sealed record CommandOutput(TextWriter Writer);

/// <summary>
///     The commands of the sample. ConsoleAppFramework creates the class from the host's services, which the DryIoc container provides. The
///     class is internal, as ConsoleAppFramework's own types are.
/// </summary>
/// <param name="output">Where the commands write.</param>
internal sealed class SampleCommands(CommandOutput output)
{
    /// <summary>
    ///     Runs when no command is given. ConsoleAppFramework hands the root command whatever no other command matched, so it turns down
    ///     arguments it does not know instead of greeting.
    /// </summary>
    /// <param name="context">The command line as ConsoleAppFramework parsed it.</param>
    /// <returns>The exit code.</returns>
    [Command("")]
    public int Hello(ConsoleAppContext context)
    {
        if (context.CommandArguments.Length > 0)
        {
            output.Writer.WriteLine($"Unknown command: {context.CommandArguments[0]}");
            return 1;
        }

        output.Writer.WriteLine("Hello World!");
        return 0;
    }
}

/// <summary>
///     Writes the configuration the host was started with.
/// </summary>
/// <param name="configuration">The configuration of the host.</param>
/// <param name="output">Where the command writes.</param>
/// <param name="instanceThing">Registered in the DryIoc container only, so the command shows it was created by it.</param>
internal sealed class DumpCommand(IConfiguration configuration, CommandOutput output, InstanceThing instanceThing)
{
    /// <summary>
    ///     Writes the configuration the host was started with.
    /// </summary>
    [Command("dump")]
    public void Dump()
    {
        output.Writer.WriteLine($"From: {instanceThing.From}");
        foreach (var item in configuration.AsEnumerable().Reverse())
        {
            output.Writer.WriteLine($"{item.Key}: {item.Value ?? ""}");
        }
    }
}
