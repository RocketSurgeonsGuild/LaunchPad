using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;
using System.Linq;

[assembly: Convention(typeof(MediatRConvention))]

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions
{
    /// <summary>
    /// MediatRConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    public class MediatRConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            var serviceConfig = context.GetOrAdd(() => new MediatRServiceConfiguration());
            context.Set(serviceConfig);

            var assemblies = context.AssemblyCandidateFinder
               .GetCandidateAssemblies(nameof(MediatR))
               .ToArray();

            ServiceRegistrar.AddRequiredServices(services, serviceConfig);
            ServiceRegistrar.AddMediatRClasses(services, assemblies);
        }
    }
}