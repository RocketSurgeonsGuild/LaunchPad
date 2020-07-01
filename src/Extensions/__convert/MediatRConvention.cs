using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.MediatR;

[assembly: Convention(typeof(MediatRConvention))]

namespace Rocket.Surgery.Conventions.MediatR
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
        public void Register(IServiceConventionContext context)
        {
            var serviceConfig = context.GetOrAdd(() => new MediatRServiceConfiguration());
            context.UseMediatR(serviceConfig);
        }
    }
}