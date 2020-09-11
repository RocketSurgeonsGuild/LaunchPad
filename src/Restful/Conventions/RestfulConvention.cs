using Hellang.Middleware.ProblemDetails;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Restful.Composition;
using Rocket.Surgery.LaunchPad.Restful.Conventions;
using Rocket.Surgery.LaunchPad.Restful.Problems;

[assembly: Convention(typeof(RestfulConvention))]

namespace Rocket.Surgery.LaunchPad.Restful.Conventions
{
    /// <summary>
    /// ProblemDetailsConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    public class RestfulConvention : IServiceConvention
    {
        /// <inheritdoc />
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services
               .AddControllers()
               .AddControllersAsServices();

            services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, RestfulApiApplicationModelProvider>());
        }
    }
}