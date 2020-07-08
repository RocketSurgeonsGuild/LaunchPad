using JetBrains.Annotations;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Serilog
{
    /// <summary>
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{ISerilogConventionContext}" />
    [PublicAPI] 
    public interface ISerilogConvention : IConvention<ISerilogConventionContext> { }
}