using Grpc.AspNetCore.Server;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    public static class GrpcServiceOptionsHelper
    {
        public static GrpcServiceOptions EnableMessageValidation(this GrpcServiceOptions options)
        {
            options.Interceptors.Add<ValidationInterceptor>();
            return options;
        }
    }
}