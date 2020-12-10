using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.ComponentModel.DataAnnotations;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation
{
    internal class ValidationInterceptor : Interceptor
    {
        private readonly IValidatorFactory _factory;
        private readonly IValidatorErrorMessageHandler _handler;
        private readonly IServiceProvider _serviceProvider;

        public ValidationInterceptor(IValidatorFactory factory, IValidatorErrorMessageHandler handler, IServiceProvider serviceProvider)
        {
            _factory = factory;
            _handler = handler;
            _serviceProvider = serviceProvider;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (_factory.GetValidator<TRequest>() is { } validator)
            {
                var validationContext = new ValidationContext<TRequest>(request);
                validationContext.SetServiceProvider(_serviceProvider);

                var results = await validator.ValidateAsync(validationContext, context.CancellationToken).ConfigureAwait(false);

                if (results.IsValid || !results.Errors.Any())
                {
                    return await continuation(request, context);
                }

                var message = await _handler.HandleAsync(results.Errors);
                throw CreateException(results, message);
            }
            return await continuation(request, context);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation
        )
        {
            if (_factory.GetValidator<TRequest>() is { } validator)
            {
                var results = validator.Validate(request);

                if (results.IsValid || !results.Errors.Any())
                {
                    return continuation(request, context);
                }

                var message = _handler.Handle(results.Errors);
                throw CreateException(results, message);
            }
            return continuation(request, context);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            BlockingUnaryCallContinuation<TRequest, TResponse> continuation
        )
        {
            if (_factory.GetValidator<TRequest>() is { } validator)
            {
                var results = validator.Validate(request);

                if (results.IsValid || !results.Errors.Any())
                {
                    return continuation(request, context);
                }

                var message = _handler.Handle(results.Errors);
                throw CreateException(results, message);
            }
            return continuation(request, context);
        }

        private RpcException CreateException(ValidationResult results, string? message)
        {
            var validationMetadata = results.Errors.ToValidationMetadata();
            validationMetadata.Add("title", "Unprocessable Entity");
            validationMetadata.Add("link", "https://tools.ietf.org/html/rfc4918#section-11.2");
            if (message is {}) validationMetadata.Add("message", message);
            throw new RpcException(new Status(StatusCode.InvalidArgument, message), validationMetadata, message);
        }
    }
}