using Hellang.Middleware.ProblemDetails;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Restful.Conventions;
using Rocket.Surgery.LaunchPad.Restful.Problems;

[assembly: Convention(typeof(ProblemDetailsConvention))]

namespace Rocket.Surgery.LaunchPad.Restful.Conventions
{
    /// <summary>
    /// ProblemDetailsConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    public class ProblemDetailsConvention : IServiceConvention
    {
        /// <inheritdoc />
        public void Register(IServiceConventionContext context)
        {
            context.Services.AddProblemDetails();
            context.Services.AddSingleton<IStartupFilter, ProblemDetailsStartupFilter>();
        }
    }
}