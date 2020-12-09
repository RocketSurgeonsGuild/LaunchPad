using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Grpc.Validation;

namespace Rocket.Surgery.LaunchPad.Grpc
{
    internal class NotFoundInterceptor : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (NotFoundException exception)
            {
                throw new RpcException(new Status(StatusCode.NotFound, exception.Message), exception.Message);
            }
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation
        )
        {
            try
            {
                return continuation(request, context);
            }
            catch (NotFoundException exception)
            {
                throw new RpcException(new Status(StatusCode.NotFound, exception.Message), exception.Message);
            }
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            BlockingUnaryCallContinuation<TRequest, TResponse> continuation
        )
        {
            try
            {
                return continuation(request, context);
            }
            catch (NotFoundException exception)
            {
                throw new RpcException(new Status(StatusCode.NotFound, exception.Message), exception.Message);
            }
        }
    }
}