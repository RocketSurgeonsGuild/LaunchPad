using System.Reflection;
using System.Threading;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

/// <summary>
///     Returns assembly information for the given application
/// </summary>
[ExtendObjectType(OperationTypeNames.Query)]
[PublicAPI]
public class AssemblyInfoQuery
{
    /// <summary>
    ///     Get the assembly version information
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public AssemblyInfo Version(IResolverContext context, CancellationToken cancellationToken)
    {
        return new(
            context.Services.GetService<FoundationOptions>()?.EntryAssembly ?? Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()
        );
    }
}
