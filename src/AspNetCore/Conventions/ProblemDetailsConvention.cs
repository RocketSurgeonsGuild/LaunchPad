using Hellang.Middleware.ProblemDetails;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

[assembly: Convention(typeof(ProblemDetailsConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions
{
    /// <summary>
    /// ProblemDetailsConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    [AfterConvention(typeof(AspNetCoreConvention))]
    public class ProblemDetailsConvention : IServiceConvention
    {
        /// <inheritdoc />
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services.AddProblemDetails();
        }
    }
}