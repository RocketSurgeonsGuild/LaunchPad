using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    /// <summary>
    /// Service collection helpers with grpc
    /// </summary>
    public static class ServiceCollectionHelper
    {
        /// <summary>
        /// Add default component for validating grpc messages
        /// </summary>
        /// <param name="services">service collection</param>
        /// <returns>service collection</returns>
        public static IServiceCollection AddGrpcValidation(this IServiceCollection services)
        {
            if (services.All(r => r.ServiceType != typeof(IValidatorErrorMessageHandler)))
                services.AddSingleton<IValidatorErrorMessageHandler, DefaultErrorMessageHandler>();

            return services;
        }
    }
}