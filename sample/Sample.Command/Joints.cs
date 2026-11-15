using ConsoleAppFramework;

using DryIoc;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

namespace Sample.Command;

/// <summary>
///     Registers the commands. ConsoleAppFramework reads the <c>Add</c> calls in the application's own compilation, so the joint that adds
///     commands lives in the application.
/// </summary>
[DovetailExport]
internal sealed class CommandsJoint : IConsoleAppJoint
{
    public void Register(IDovetailContext context, ConsoleApp.ConsoleAppBuilder app)
    {
        app.Add<SampleCommands>();
        app.Add<DumpCommand>();
    }
}

/// <summary>
///     Registers what only the DryIoc container knows: Joinery.Dovetail.DryIoc builds the host's service provider from DryIoc and hands each
///     <see cref="IDryIocJoint" /> the container.
/// </summary>
[DovetailExport]
internal sealed class DryIocSetup : IDryIocJoint
{
    public void Register(IDovetailContext context, IContainer container) => container.Use(new InstanceThing());
}
