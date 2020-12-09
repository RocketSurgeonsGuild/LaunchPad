using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Grpc.Conventions;
using Rocket.Surgery.LaunchPad.Grpc.Validation;

[assembly: Convention(typeof(GrpcConvention))]

namespace Rocket.Surgery.LaunchPad.Grpc.Conventions
{
    /// <summary>
    /// ProblemDetailsConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    public class GrpcConvention : IServiceConvention
    {
        /// <inheritdoc />
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services
               .AddGrpcValidation()
               .AddGrpc(options =>
                    {
                        options.EnableMessageValidation();
                        options.Interceptors.Add<NotFoundInterceptor>();
                    }
                )
               ;
        }
    }
}