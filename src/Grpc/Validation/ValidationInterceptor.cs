using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    internal class ValidationInterceptor : Interceptor
    {
        private readonly IValidatorLocator _locator;
        private readonly IValidatorErrorMessageHandler _handler;

        public ValidationInterceptor(IValidatorLocator locator, IValidatorErrorMessageHandler handler)
        {
            _locator = locator;
            _handler = handler;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (_locator.TryGetValidator<TRequest>(out var validator))
            {
                var results = await validator.ValidateAsync(request);

                if (results.IsValid || !results.Errors.Any())
                {
                    return await continuation(request, context);
                }

                var message = await _handler.HandleAsync(results.Errors);
                var validationMetadata = results.Errors.ToValidationMetadata();
                throw new RpcException(new Status(StatusCode.InvalidArgument, message), validationMetadata);
            }
            return await continuation(request, context);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation
        )
        {
            if (_locator.TryGetValidator<TRequest>(out var validator))
            {
                var results = validator.Validate(request);

                if (results.IsValid || !results.Errors.Any())
                {
                    return continuation(request, context);
                }

                var message = _handler.Handle(results.Errors);
                var validationMetadata = results.Errors.ToValidationMetadata();
                throw new RpcException(new Status(StatusCode.InvalidArgument, message), validationMetadata);
            }
            return continuation(request, context);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            BlockingUnaryCallContinuation<TRequest, TResponse> continuation
        )
        {
            if (_locator.TryGetValidator<TRequest>(out var validator))
            {
                var results = validator.Validate(request);

                if (results.IsValid || !results.Errors.Any())
                {
                    return continuation(request, context);
                }

                var message = _handler.Handle(results.Errors);
                var validationMetadata = results.Errors.ToValidationMetadata();
                throw new RpcException(new Status(StatusCode.InvalidArgument, message), validationMetadata);
            }
            return continuation(request, context);
        }
    }
}