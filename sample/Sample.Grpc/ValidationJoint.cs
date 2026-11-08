using Grpc.AspNetCore.Server;
using Grpc.Core;
using Grpc.Core.Interceptors;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Rocket.Surgery.LaunchPad.Grpc.Joints;

using ZeroAlloc.Mediator.Validation;

namespace Sample.Grpc;

/// <summary>
///     Turns the <see cref="ValidationFailedException" /> the ZeroAlloc mediator throws for an invalid request into an
///     <see cref="StatusCode.InvalidArgument" /> status that names the failed properties.
/// </summary>
[DovetailExport]
[AfterJoint<GrpcJoint>]
public class ValidationJoint : IServiceJoint
{
    public void Register(IDovetailContext context, IServiceCollection services) =>
        services.Configure<GrpcServiceOptions>(options => options.Interceptors.Add<ValidationInterceptor>());

    private sealed class ValidationInterceptor : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation
        )
        {
            try
            {
                return await continuation(request, context).ConfigureAwait(false);
            }
            catch (ValidationFailedException exception)
            {
                throw ToRpcException(exception);
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation
        )
        {
            try
            {
                await continuation(request, responseStream, context).ConfigureAwait(false);
            }
            catch (ValidationFailedException exception)
            {
                throw ToRpcException(exception);
            }
        }

        private static RpcException ToRpcException(ValidationFailedException exception)
        {
            var failures = exception.Error.Failures;
            var trailers = new Metadata();
            foreach (var failure in failures)
            {
                trailers.Add(failure.PropertyName.ToLowerInvariant(), failure.ErrorMessage);
            }

            var detail = string.Join(Environment.NewLine, failures.Select(failure => $"Property {failure.PropertyName} failed validation. {failure.ErrorMessage}"));
            return new(new(StatusCode.InvalidArgument, detail), trailers);
        }
    }
}
