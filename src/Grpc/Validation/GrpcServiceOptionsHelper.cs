using Grpc.AspNetCore.Server;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    /// <summary>
    /// Helper for configuration validation with Grpc
    /// </summary>
    public static class GrpcServiceOptionsHelper
    {
        /// <summary>
        /// Enable message validation
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static GrpcServiceOptions EnableMessageValidation(this GrpcServiceOptions options)
        {
            options.Interceptors.Add<ValidationInterceptor>();
            return options;
        }
    }
}