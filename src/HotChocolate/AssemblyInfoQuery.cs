using System.Reflection;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

/// <summary>
///     Returns assembly information for the given application
/// </summary>
[PublicAPI]
[ExtendObjectType(OperationTypeNames.Query)]
public class AssemblyInfoQuery
{
    /// <summary>
    ///     Get the assembly version information
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
#pragma warning disable CA1822
    public AssemblyInfo Version(IResolverContext context, CancellationToken cancellationToken)
#pragma warning restore CA1822
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        return new AssemblyInfo(Assembly.GetEntryAssembly()!);
    }
}
